using AutoMapper;
using Evangelion01.BackendApp.Infrastructure;
using Evangelion01.Contracts;
using Evangelion01.Contracts.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evangelion01.BackendApp.Functions.Students
{
    public interface IStudentService
    {
        Task<WrappedResponse> Get(GetStudentModel model);
        Task<WrappedResponse> GetAll(GetAllStudentModel model);
        Task<WrappedResponse> Add(AddStudentModel model);
        Task<WrappedResponse> Delete(DeleteStudentModel model);
        Task<WrappedResponse> Update(UpdateStudentModel model);
    }

    public class StudentService : IStudentService
    {
        private readonly ILogger<StudentService> _logger;
        private readonly IMapper _mapper;

        private readonly Container _studentsContainer;

        public StudentService(ILogger<StudentService> logger, IMapper mapper, CosmosClient cosmosClient)
        {
            _logger = logger;
            _mapper = mapper;

            // get DB container
            var database = cosmosClient.GetDatabase(EnvironmentConfig.DatabaseName);
            _studentsContainer = database.GetContainer(Student.Container);
        }

        public Task<WrappedResponse> Get(GetStudentModel model)
        {
            // find the doc
            var student = _studentsContainer
                .GetItemLinqQueryable<Grade>()
                .FirstOrDefault((doc) => doc.StudentId == model.StudentId);

            // doc not found, return
            if (student == null)
            {
                _logger.LogInformation("Student not found, ID: {0}", model.StudentId);
                return Task.FromResult(new WrappedResponse(false, "Record not found"));
            }

            // return the doc
            _logger.LogInformation("Get student, ID: {0}", model.StudentId);
            return Task.FromResult(new WrappedResponse(true, "Student deleted", student));
        }

        public async Task<WrappedResponse> GetAll(GetAllStudentModel model)
        {
            // flatten IAsyncEnumerable
            var resultset = await InternalGetAll(model).ToListAsync();

            // return the list
            _logger.LogInformation("Get grade list, record count: {0}", resultset.Count);
            return new WrappedResponse(true, "Successfully retrieve grades", resultset);
        }

        public async Task<WrappedResponse> Add(AddStudentModel model)
        {
            // map the grade entity
            var entity = _mapper.Map<Student>(model);

            // create the doc
            var result = await _studentsContainer.CreateItemAsync(entity);

            // return the doc
            _logger.LogInformation("Create student, ID: {0}", result.Resource.StudentId);
            return new WrappedResponse(true, "Student added successfully", _mapper.Map<StudentDto>(result.Resource));
        }

        public async Task<WrappedResponse> Delete(DeleteStudentModel model)
        {
            // find the doc
            var student = _studentsContainer
                .GetItemLinqQueryable<Student>()
                .FirstOrDefault((doc) => doc.StudentId == model.StudentId);

            // doc not found, return
            if (student == null)
            {
                return new WrappedResponse(false, "Record not found");
            }

            // delete doc
            await _studentsContainer.DeleteItemAsync<Student>(model.StudentId, new PartitionKey("id"));
            return new WrappedResponse(true, "Grade deleted");
        }

        public Task<WrappedResponse> Update(UpdateStudentModel model)
        {
            throw new NotImplementedException();
        }

        // --- Private Methods

        private async IAsyncEnumerable<StudentDto> InternalGetAll(GetAllStudentModel model)
        {
            _logger.LogDebug("Querying students\n{0}", JsonConvert.SerializeObject(model));

            // filter out comments based on fields
            var query = _studentsContainer.GetItemLinqQueryable<Student>().AsQueryable();
            if (!string.IsNullOrWhiteSpace(model.Keyword))
            {
                query = query.Where(x => x.Name.Contains(model.Keyword));
            }
            if (model.Group != null)
            {
                query = query.Where(x => x.Group == model.Group);
            }

            // calculate offset for pagination
            query = query.Skip((model.Page - 1) * model.Limit).Take(model.Limit);
            using var iterator = query.ToFeedIterator();

            // while the iterator has data...
            while (iterator.HasMoreResults)
            {
                // read the next available data
                var resultSet = await iterator.ReadNextAsync();
                foreach (var item in resultSet)
                {
                    // project the data into DTO
                    yield return _mapper.Map<StudentDto>(item);
                }
            }
        }
    }
}
