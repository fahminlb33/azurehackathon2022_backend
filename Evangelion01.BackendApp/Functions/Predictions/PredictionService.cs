using Evangelion01.BackendApp.Infrastructure;
using Evangelion01.Contracts;
using Evangelion01.Contracts.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Evangelion01.BackendApp.Functions.Predictions
{
    public interface IPredictionService
    {
        Task<WrappedResponse> PredictStudent(PredictionModel model);
    }

    public class PredictionService : IPredictionService
    {
        private readonly ILogger<PredictionService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly Container _usersContainer;
        private readonly Container _gradesContainer;

        public PredictionService(ILogger<PredictionService> logger, IHttpClientFactory httpClientFactory, CosmosClient cosmosClient)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;

            // get DB container
            var database = cosmosClient.GetDatabase(EnvironmentConfig.DatabaseName);
            _usersContainer = database.GetContainer(Student.Container);
            _gradesContainer = database.GetContainer(Grade.Container);
        }

        public async Task<WrappedResponse> PredictStudent(PredictionModel model)
        {
            // find the user
            var user = _usersContainer
               .GetItemLinqQueryable<Student>()
               .FirstOrDefault((doc) => doc.StudentId == model.UserId);

            // if no user is found, return
            if (user == null)
            {
                return new WrappedResponse(false, "User not found");
            }

            // get all grades associated with this user
            var grades = _gradesContainer
                .GetItemLinqQueryable<Grade>()
                .Where((doc) => doc.StudentId == user.StudentId)
                .ToList();
            var (predictionFeatures, usedData) = GradesToArray(grades);

            // run prediction for single item
            var client = _httpClientFactory.CreateClient();
            var predictionResult = await client.PostAsJsonAsync(EnvironmentConfig.PredictionUri, new
            {
                data = new[] { predictionFeatures }
            });
            if (!predictionResult.IsSuccessStatusCode)
            {
                _logger.LogError("Cannot run prediction: " + predictionResult.ReasonPhrase);
                return new WrappedResponse(false, "Cannot run prediction");
            }

            // get the prediction result array
            var predictionArray = await predictionResult.Content.ReadAsAsync<List<string>>();

            // return the prediction result
            var dto = new PredictionDto
            {
                Predicted = predictionArray.First(),
                Grades = usedData
            };
            return new WrappedResponse(true, "Prediction run successfully", dto);
        }

        private (List<double> predictionFeatures, Dictionary<GradeSubject, double> inputData)  GradesToArray(IEnumerable<Grade> grades)
        {
            var gradeGrouped = grades
                .GroupBy((doc) => doc.Subject)
                .ToDictionary(x => x.Key, y => (double)y.Sum(col => col.Score) / Math.Max(y.Count(), 1));
            var features = new List<double>
            {
                gradeGrouped.GetValueOrDefault(GradeSubject.Anthropology, 0),
                gradeGrouped.GetValueOrDefault(GradeSubject.Biology, 0),
                gradeGrouped.GetValueOrDefault(GradeSubject.Economy, 0),
                gradeGrouped.GetValueOrDefault(GradeSubject.Physics, 0),
                gradeGrouped.GetValueOrDefault(GradeSubject.Geography, 0),
                gradeGrouped.GetValueOrDefault(GradeSubject.BahasaIndonesia, 0),
                gradeGrouped.GetValueOrDefault(GradeSubject.EnglishLanguage, 0),
                gradeGrouped.GetValueOrDefault(GradeSubject.JapaneseLanguage, 0),
                gradeGrouped.GetValueOrDefault(GradeSubject.GermanLanguage, 0),
                gradeGrouped.GetValueOrDefault(GradeSubject.Chemistry, 0),
                gradeGrouped.GetValueOrDefault(GradeSubject.Mathematics, 0),
                gradeGrouped.GetValueOrDefault(GradeSubject.AdvanceMathematics, 0),
                gradeGrouped.GetValueOrDefault(GradeSubject.Religion, 0),
                gradeGrouped.GetValueOrDefault(GradeSubject.PhysicalEducation, 0),
                gradeGrouped.GetValueOrDefault(GradeSubject.CivicEducation, 0),
                gradeGrouped.GetValueOrDefault(GradeSubject.Crafts, 0),
                gradeGrouped.GetValueOrDefault(GradeSubject.History, 0),
                gradeGrouped.GetValueOrDefault(GradeSubject.IndonesianHistory, 0),
                gradeGrouped.GetValueOrDefault(GradeSubject.IndonesianLiterature, 0),
                gradeGrouped.GetValueOrDefault(GradeSubject.EnglishLiterature, 0),
                gradeGrouped.GetValueOrDefault(GradeSubject.Sociology, 0),
                gradeGrouped.GetValueOrDefault(GradeSubject.BahasaSunda, 0),
            };

            return (features, gradeGrouped);
        }
    }
}
