using AutoMapper;
using Evangelion01.BackendApp.Infrastructure;
using Evangelion01.Contracts;
using Evangelion01.Contracts.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evangelion01.BackendApp.Functions.Grades
{
    public interface IGradeService
    {
        Task<WrappedResponse> GetAll(string studentId);
        Task<WrappedResponse> Add(AddGradeModel model);
    }

    public class GradeService : IGradeService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<GradeService> _logger;
        private readonly IMapper _mapper;

        private readonly Container _gradesContainer;

        public GradeService(IConfiguration config, ILogger<GradeService> logger, IMapper mapper, CosmosClient cosmosClient)
        {
            _config = config;
            _logger = logger;
            _mapper = mapper;

            // get DB container
            var database = cosmosClient.GetDatabase(_config[SettingsKey.DatabaseName]);
            _gradesContainer = database.GetContainer(Grade.Container);
        }

        public async Task<WrappedResponse> Add(AddGradeModel model)
        {
            var entity = _mapper.Map<Grade>(model);
            entity.GradeId = Guid.NewGuid().ToString();
            entity.Categories = EntityValueConverter.GetCategoriesFromSubject(model.Subject).ToList();

            var result = await _gradesContainer.CreateItemAsync(entity);
            return new WrappedResponse(true, "Grade added successfully.", _mapper.Map<GradeDto>(result.Resource));
        }

        public async Task<WrappedResponse> GetAll(string studentId)
        {
            var iterable = await InternalGetAll(studentId).ToListAsync();
            return new WrappedResponse(true, "Berhasil mengambil data komentar.", iterable);
        }

        private async IAsyncEnumerable<GradeDto> InternalGetAll(string studentId)
        {
            // filter out comments based on the post slug
            var query = new QueryDefinition($"SELECT * FROM c WHERE StringEquals(c.StudentId, \"{studentId}\")");
            var commentIterator = _gradesContainer.GetItemQueryIterator<Grade>(query);

            // while the iterator has data...
            while (commentIterator.HasMoreResults)
            {
                // read the next available data
                var resultSet = await commentIterator.ReadNextAsync();
                foreach (var item in resultSet)
                {
                    // project the data into DTO
                    yield return _mapper.Map<GradeDto>(item);
                }
            }
        }
    }
}
