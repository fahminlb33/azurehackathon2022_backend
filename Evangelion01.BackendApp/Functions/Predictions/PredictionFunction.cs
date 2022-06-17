using System;
using System.Net;
using System.Threading.Tasks;
using Evangelion01.BackendApp.Infrastructure.Helpers;
using Evangelion01.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Evangelion01.BackendApp.Functions.Predictions
{
    public class PredictionFunction
    {
        public const string PredictionFunctionTag = "Predictions API";

        private readonly ILogger<PredictionFunction> _logger;
        private readonly IPredictionService _predictionService;

        public PredictionFunction(ILogger<PredictionFunction> log, IPredictionService predictionService)
        {
            _logger = log;
            _predictionService = predictionService;
        }

        [FunctionName("PredictionFunction_PredictStudent")]
        [OpenApiOperation(operationId: "PredictionFunction_PredictStudent", tags: new[] { PredictionFunctionTag })]
        [OpenApiParameter("userId", In = ParameterLocation.Path, Type = typeof(string), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: Constants.ContentTypeJson, bodyType: typeof(WrappedResponse<PredictionDto>))]
        [OpenApiSecurity(Constants.OpenApiBearer, SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = Constants.OpenApJwt)]
        public async Task<IActionResult> PredictStudent([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "predictions/{studentId}")] HttpRequest req, string studentId)
        {
            try
            {
                // get model object
                var model = new PredictionModel
                {
                    StudentId = studentId
                };

                // validate model
                var validationResult = await Helpers.ValidateAsync<PredictionModel, PredictionModel.ValidatorClass>(model);
                if (!validationResult.IsValid)
                {
                    return validationResult.ToBadRequest();
                }

                // run handler
                var response = await _predictionService.PredictStudent(validationResult.Value!);
                return response.Success ? new JsonResult(response) : new ConflictObjectResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when processing the request");
                return new BadRequestResult();
            }
        }
    }
}

