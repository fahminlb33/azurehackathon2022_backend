using AutoMapper;
using Evangelion01.BackendApp.Infrastructure;
using Evangelion01.BackendApp.Infrastructure.Helpers;
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

        public async Task<WrappedResponse> Get(GetStudentModel model)
        {
            // find the doc
            var sql = $"SELECT * FROM c WHERE StringEquals(c.id, \"{model.StudentId}\")";
            var student = await _studentsContainer.GetFirstOrDefault<Student>(sql);

            // doc not found, return
            if (student == null)
            {
                _logger.LogInformation("Student not found, ID: {0}", model.StudentId);
                return new WrappedResponse(false, "Record not found");
            }

            // return the doc
            _logger.LogInformation("Get student, ID: {0}", model.StudentId);
            return new WrappedResponse(true, "Student deleted", student);
        }

        public async Task<WrappedResponse> GetAll(GetAllStudentModel model)
        {
            // flatten IAsyncEnumerable
            var resultset = new List<StudentDto>();

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

            // calculate total items
            var totalFiltered = await query.CountAsync();

            // calculate offset for pagination
            query = query.Skip((model.Page - 1) * model.Limit).Take(model.Limit);
            using var iterator = query.ToFeedIterator();

            // while the iterator has data...
            _logger.LogDebug("Querying students\n{0}", JsonConvert.SerializeObject(model));
            while (iterator.HasMoreResults)
            {
                // read the next available data
                var resultSet = await iterator.ReadNextAsync();
                foreach (var item in resultSet)
                {
                    // project the data into DTO
                    resultset.Add(_mapper.Map<StudentDto>(item));
                }
            }

            var dto = new StudentListDto
            {
                CurrentPage = model.Page,
                TotalDataOnPage = resultset.Count,
                TotalData = totalFiltered,
                TotalPage = Convert.ToInt32(Math.Ceiling((double)totalFiltered / model.Limit)),
                Records = resultset
            };

            // return the list
            _logger.LogInformation("Get grade list, record count: {0}", resultset.Count);
            return new WrappedResponse(true, "Successfully retrieve grades", dto);
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
            var sql = $"SELECT * FROM c WHERE StringEquals(c.id, \"{model.StudentId}\")";
            var student = await _studentsContainer.GetFirstOrDefault<Student>(sql);

            // doc not found, return
            if (student == null)
            {
                return new WrappedResponse(false, "Record not found");
            }

            // delete doc
            await _studentsContainer.DeleteItemAsync<Student>(model.StudentId, new PartitionKey("id"));
            return new WrappedResponse(true, "Grade deleted");
        }

        public async Task<WrappedResponse> Update(UpdateStudentModel model)
        {
            // find the doc
            var sql = $"SELECT * FROM c WHERE StringEquals(c.id, \"{model.StudentId}\")";
            var student = await _studentsContainer.GetFirstOrDefault<Student>(sql);

            // doc not found, return
            if (student == null)
            {
                return new WrappedResponse(false, "Record not found");
            }

            // change feilds
            student.Group = model.Group;
            if (model.Name != null)
            {
                student.Name = model.Name;
            }

            // save item
            var result = await _studentsContainer.ReplaceItemAsync(student, student.StudentId);

            // return the doc
            _logger.LogInformation("Update student, ID: {0}", result.Resource.StudentId);
            return new WrappedResponse(true, "Student updated", result.Resource);
        }
    }
}
