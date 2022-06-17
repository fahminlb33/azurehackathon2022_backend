using System;
using System.Net;
using System.Threading.Tasks;
using Evangelion01.BackendApp.Functions.Grades;
using Evangelion01.BackendApp.Infrastructure.Authentication;
using Evangelion01.BackendApp.Infrastructure.Helpers;
using Evangelion01.Contracts;
using Evangelion01.Contracts.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Evangelion01.Functions.Grades
{
    public class GradeFunction
    {
        public const string GradesFunctionTag = "Grades API";

        private readonly ILogger<GradeFunction> _logger;
        private readonly IGradeService _gradeService;
                
        public GradeFunction(ILogger<GradeFunction> log, IGradeService gradeService)
        {
            _logger = log;
            _gradeService = gradeService;
        }

        [Authorize]
        [FunctionName("GradesFunction_Get")]
        [OpenApiOperation(operationId: "GradesFunction_Get", tags: new[] { GradesFunctionTag })]
        [OpenApiParameter("gradeId", In = ParameterLocation.Path, Type = typeof(string), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: Constants.ContentTypeJson, bodyType: typeof(WrappedResponse<GradeDto[]>))]
        [OpenApiSecurity(Constants.OpenApiBearer, SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = Constants.OpenApJwt)]
        public async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "grades/{gradeId}")] HttpRequest req, string gradeId)
        {
            try
            {
                // get model object
                var model = new GetGradeModel
                {
                    GradeId = gradeId
                };

                // validate model
                var validationResult = await Helpers.ValidateAsync<GetGradeModel, GetGradeModel.ValidatorClass>(model);
                if (!validationResult.IsValid)
                {
                    return validationResult.ToBadRequest();
                }

                // run handler
                var response = await _gradeService.Get(validationResult.Value!);
                return response.Success ? new JsonResult(response) : new ConflictObjectResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when processing the request");
                return new BadRequestResult();
            }
        }

        [Authorize]
        [FunctionName("GradesFunction_GetAll")]
        [OpenApiOperation(operationId: "GradesFunction_GetAll", tags: new[] { GradesFunctionTag })]
        [OpenApiParameter("userId", In = ParameterLocation.Query, Type = typeof(string), Required = false)]
        [OpenApiParameter("subject", In = ParameterLocation.Query, Type = typeof(GradeSubject), Required = false)]
        [OpenApiParameter("semester", In = ParameterLocation.Query, Type = typeof(int), Required = false)]
        [OpenApiParameter("page", In = ParameterLocation.Query, Type = typeof(int), Required = false)]
        [OpenApiParameter("limit", In = ParameterLocation.Query, Type = typeof(int), Required = false)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: Constants.ContentTypeJson, bodyType: typeof(WrappedResponse<GradeDto[]>))]
        [OpenApiSecurity(Constants.OpenApiBearer, SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = Constants.OpenApJwt)]
        public async Task<IActionResult> GetAll([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "grades")] HttpRequest req)
        {
            try
            {
                // get model object
                var model = await req.GetJsonQuery<GetAllGradeModel>();

                // validate model
                var validationResult = await Helpers.ValidateAsync<GetAllGradeModel, GetAllGradeModel.ValidatorClass>(model);
                if (!validationResult.IsValid)
                {
                    return validationResult.ToBadRequest();
                }

                // run handler
                var response = await _gradeService.GetAll(validationResult.Value!);
                return response.Success ? new JsonResult(response) : new ConflictObjectResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when processing the request");
                return new BadRequestResult();
            }
        }

        [Authorize]
        [FunctionName("GradesFunction_Add")]
        [OpenApiOperation(operationId: "GradesFunction_Add", tags: new[] { GradesFunctionTag })]
        [OpenApiRequestBody(Constants.ContentTypeJson, typeof(AddGradeModel), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: Constants.ContentTypeJson, bodyType: typeof(WrappedResponse<GradeDto>))]
        [OpenApiSecurity(Constants.OpenApiBearer, SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = Constants.OpenApJwt)]
        public async Task<IActionResult> Add([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "grades")] HttpRequest req)
        {
            try
            {
                // get model object
                var model = await req.GetJsonBody<AddGradeModel>();

                // validate model
                var validationResult = await Helpers.ValidateAsync<AddGradeModel, AddGradeModel.ValidatorClass>(model);
                if (!validationResult.IsValid)
                {
                    return validationResult.ToBadRequest();
                }

                // run handler
                var response = await _gradeService.Add(validationResult.Value!);
                return response.Success ? new JsonResult(response) : new ConflictObjectResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when processing the request");
                return new BadRequestResult();
            }
        }

        [Authorize]
        [FunctionName("GradesFunction_Delete")]
        [OpenApiOperation(operationId: "GradesFunction_Delete", tags: new[] { GradesFunctionTag })]
        [OpenApiParameter("gradeId", In = ParameterLocation.Path, Type = typeof(string), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: Constants.ContentTypeJson, bodyType: typeof(WrappedResponse))]
        [OpenApiSecurity(Constants.OpenApiBearer, SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = Constants.OpenApJwt)]
        public async Task<IActionResult> Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "grades/{gradeId}")] HttpRequest req, string gradeId)
        {
            try
            {
                // get model object
                var model = new DeleteGradeModel
                {
                    GradeId = gradeId,
                };

                // validate model
                var validationResult = await Helpers.ValidateAsync<DeleteGradeModel, DeleteGradeModel.ValidatorClass>(model);
                if (!validationResult.IsValid)
                {
                    return validationResult.ToBadRequest();
                }

                // run handler
                var response = await _gradeService.Delete(validationResult.Value!);
                return response.Success ? new JsonResult(response) : new ConflictObjectResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when processing the request");
                return new BadRequestResult();
            }
        }

        [Authorize]
        [FunctionName("GradesFunction_Update")]
        [OpenApiOperation(operationId: "GradesFunction_Update", tags: new[] { GradesFunctionTag })]
        [OpenApiRequestBody(Constants.ContentTypeJson, typeof(UpdateGradeModel), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: Constants.ContentTypeJson, bodyType: typeof(WrappedResponse))]
        [OpenApiSecurity(Constants.OpenApiBearer, SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = Constants.OpenApJwt)]
        public async Task<IActionResult> Update([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "grades")] HttpRequest req)
        {
            try
            {
                // get model object
                var model = await req.GetJsonBody<UpdateGradeModel>();

                // validate model
                var validationResult = await Helpers.ValidateAsync<UpdateGradeModel, UpdateGradeModel.ValidatorClass>(model);
                if (!validationResult.IsValid)
                {
                    return validationResult.ToBadRequest();
                }

                // run handler
                var response = await _gradeService.Update(validationResult.Value!);
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

