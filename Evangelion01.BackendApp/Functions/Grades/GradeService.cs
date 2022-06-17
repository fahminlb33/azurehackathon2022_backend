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

namespace Evangelion01.BackendApp.Functions.Grades
{
    public interface IGradeService
    {
        Task<WrappedResponse> Get(GetGradeModel model);
        Task<WrappedResponse> GetAll(GetAllGradeModel model);
        Task<WrappedResponse> Add(AddGradeModel model);
        Task<WrappedResponse> Delete(DeleteGradeModel model);
        Task<WrappedResponse> Update(UpdateGradeModel model);
    }

    public class GradeService : IGradeService
    {
        private readonly ILogger<GradeService> _logger;
        private readonly IMapper _mapper;

        private readonly Container _gradesContainer;

        public GradeService(ILogger<GradeService> logger, IMapper mapper, CosmosClient cosmosClient)
        {
            _logger = logger;
            _mapper = mapper;

            // get DB container
            var database = cosmosClient.GetDatabase(EnvironmentConfig.DatabaseName);
            _gradesContainer = database.GetContainer(Grade.Container);
        }

        public Task<WrappedResponse> Get(GetGradeModel model)
        {
            // find the doc
            var grade = _gradesContainer
                .GetItemLinqQueryable<Grade>()
                .FirstOrDefault((doc) => doc.GradeId == model.GradeId);

            // doc not found, return
            if (grade == null)
            {
                _logger.LogInformation("Grade not found, ID: {0}", model.GradeId);
                return Task.FromResult(new WrappedResponse(false, "Record not found"));
            }

            // return the doc
            _logger.LogInformation("Get grade, ID: {0}", model.GradeId);
            return Task.FromResult(new WrappedResponse(true, "Grade deleted", grade));
        }

        public async Task<WrappedResponse> GetAll(GetAllGradeModel model)
        {
            // flatten IAsyncEnumerable
            var resultset = await InternalGetAll(model).ToListAsync();

            // return the list
            _logger.LogInformation("Get grade list, record count: {0}", resultset.Count);
            return new WrappedResponse(true, "Successfully retrieve grades", resultset);
        }

        public async Task<WrappedResponse> Add(AddGradeModel model)
        {
            // map the grade entity
            var entity = _mapper.Map<Grade>(model);
            entity.Categories = EntityValueConverter.GetCategoriesFromSubject(model.Subject).ToList();

            // create the doc
            var result = await _gradesContainer.CreateItemAsync(entity);

            // return the doc
            _logger.LogInformation("Create grade, ID: {0}", result.Resource.GradeId);
            return new WrappedResponse(true, "Grade added successfully", _mapper.Map<GradeDto>(result.Resource));
        }

        public async Task<WrappedResponse> Delete(DeleteGradeModel model)
        {
            // find the doc
            var grade = _gradesContainer
                 .GetItemLinqQueryable<Grade>()
                 .FirstOrDefault((doc) => doc.GradeId == model.GradeId);

            // doc not found, return
            if (grade == null)
            {
                return new WrappedResponse(false, "Record not found");
            }

            // delete doc
            await _gradesContainer.DeleteItemAsync<Grade>(model.GradeId, new PartitionKey("id"));
            return new WrappedResponse(true, "Grade deleted");
        }

        public async Task<WrappedResponse> Update(UpdateGradeModel model)
        {
            // find the doc
            var grade = _gradesContainer
                .GetItemLinqQueryable<Grade>()
                .FirstOrDefault((doc) => doc.GradeId == model.GradeId);

            // doc not found, return
            if (grade == null)
            {
                return new WrappedResponse(false, "Record not found");
            }

            // change feilds
            if (model.Subject != null)
            {
                grade.Subject = model.Subject.Value;
            }
            if (model.Semester != null)
            {
                grade.Semester = model.Semester.Value;
            }
            if (model.Score != null)
            {
                grade.Score = model.Score.Value;
            }

            // save item
            var result = await _gradesContainer.ReplaceItemAsync(grade, grade.GradeId);

            // return the doc
            _logger.LogInformation("Create grade, ID: {0}", result.Resource.GradeId);
            return new WrappedResponse(true, "Grade updated", result.Resource);
        }

        // --- Private Methods

        private async IAsyncEnumerable<GradeDto> InternalGetAll(GetAllGradeModel model)
        {
            _logger.LogDebug("Querying grade\n{0}", JsonConvert.SerializeObject(model));

            // filter out comments based on fields
            var query = _gradesContainer.GetItemLinqQueryable<Grade>().AsQueryable();
            if (!string.IsNullOrWhiteSpace(model.UserId))
            {
                query = query.Where(x => x.StudentId == model.UserId);
            }
            if (model.Subject != null)
            {
                query = query.Where(x => x.Subject == model.Subject);
            }
            if (model.Semester != null)
            {
                query = query.Where(x => x.Semester == model.Semester);
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
                    yield return _mapper.Map<GradeDto>(item);
                }
            }
        }
    }
}
