using System;
using System.Net;
using System.Threading.Tasks;
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

namespace Evangelion01.BackendApp.Functions.Students
{
    public class StudentFunction
    {
        public const string StudentsFunctionTag = "Students API";

        private readonly ILogger<StudentFunction> _logger;
        private readonly IStudentService _studentService;

        public StudentFunction(ILogger<StudentFunction> log, IStudentService studentService)
        {
            _logger = log;
            _studentService = studentService;
        }


        [Authorize]
        [FunctionName("StudentFunction_Get")]
        [OpenApiOperation(operationId: "StudentFunction_Get", tags: new[] { StudentsFunctionTag })]
        [OpenApiParameter("gradeId", In = ParameterLocation.Path, Type = typeof(string), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: Constants.ContentTypeJson, bodyType: typeof(WrappedResponse<StudentDto[]>))]
        [OpenApiSecurity(Constants.OpenApiBearer, SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = Constants.OpenApJwt)]
        public async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "students/{studentId}")] HttpRequest req, string studentId)
        {
            try
            {
                // get model object
                var model = new GetStudentModel
                {
                    StudentId = studentId
                };

                // validate model
                var validationResult = await Helpers.ValidateAsync<GetStudentModel, GetStudentModel.ValidatorClass>(model);
                if (!validationResult.IsValid)
                {
                    return validationResult.ToBadRequest();
                }

                // run handler
                var response = await _studentService.Get(validationResult.Value!);
                return response.Success ? new JsonResult(response) : new ConflictObjectResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when processing the request");
                return new BadRequestResult();
            }
        }

        [Authorize]
        [FunctionName("StudentFunction_GetAll")]
        [OpenApiOperation(operationId: "StudentFunction_GetAll", tags: new[] { StudentsFunctionTag })]
        [OpenApiParameter("keyword", In = ParameterLocation.Query, Type = typeof(string), Required = false)]
        [OpenApiParameter("group", In = ParameterLocation.Query, Type = typeof(StudentGroup), Required = false)]
        [OpenApiParameter("page", In = ParameterLocation.Query, Type = typeof(int), Required = false)]
        [OpenApiParameter("limit", In = ParameterLocation.Query, Type = typeof(int), Required = false)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: Constants.ContentTypeJson, bodyType: typeof(WrappedResponse<StudentDto[]>))]
        [OpenApiSecurity(Constants.OpenApiBearer, SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = Constants.OpenApJwt)]
        public async Task<IActionResult> GetAll([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "students")] HttpRequest req)
        {
            try
            {
                // get model object
                var model = await req.GetJsonQuery<GetAllStudentModel>();

                // validate model
                var validationResult = await Helpers.ValidateAsync<GetAllStudentModel, GetAllStudentModel.ValidatorClass>(model);
                if (!validationResult.IsValid)
                {
                    return validationResult.ToBadRequest();
                }

                // run handler
                var response = await _studentService.GetAll(validationResult.Value!);
                return response.Success ? new JsonResult(response) : new ConflictObjectResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when processing the request");
                return new BadRequestResult();
            }
        }

        [Authorize]
        [FunctionName("StudentFunction_Add")]
        [OpenApiOperation(operationId: "StudentFunction_Add", tags: new[] { StudentsFunctionTag })]
        [OpenApiRequestBody(Constants.ContentTypeJson, typeof(AddStudentModel), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: Constants.ContentTypeJson, bodyType: typeof(WrappedResponse<StudentDto>))]
        [OpenApiSecurity(Constants.OpenApiBearer, SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = Constants.OpenApJwt)]
        public async Task<IActionResult> Add([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "students")] HttpRequest req)
        {
            try
            {
                // get model object
                var model = await req.GetJsonBody<AddStudentModel>();

                // validate model
                var validationResult = await Helpers.ValidateAsync<AddStudentModel, AddStudentModel.ValidatorClass>(model);
                if (!validationResult.IsValid)
                {
                    return validationResult.ToBadRequest();
                }

                // run handler
                var response = await _studentService.Add(validationResult.Value!);
                return response.Success ? new JsonResult(response) : new ConflictObjectResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when processing the request");
                return new BadRequestResult();
            }
        }

        [Authorize]
        [FunctionName("StudentFunction_Delete")]
        [OpenApiOperation(operationId: "StudentFunction_Delete", tags: new[] { StudentsFunctionTag })]
        [OpenApiParameter("studentId", In = ParameterLocation.Path, Type = typeof(string), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: Constants.ContentTypeJson, bodyType: typeof(WrappedResponse<StudentDto>))]
        [OpenApiSecurity(Constants.OpenApiBearer, SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = Constants.OpenApJwt)]
        public async Task<IActionResult> Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "students/{studentId}")] HttpRequest req, string studentId)
        {
            try
            {
                // get model object
                var model = new DeleteStudentModel
                {
                    StudentId = studentId
                };

                // validate model
                var validationResult = await Helpers.ValidateAsync<DeleteStudentModel, DeleteStudentModel.ValidatorClass>(model);
                if (!validationResult.IsValid)
                {
                    return validationResult.ToBadRequest();
                }

                // run handler
                var response = await _studentService.Delete(validationResult.Value!);
                return response.Success ? new JsonResult(response) : new ConflictObjectResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when processing the request");
                return new BadRequestResult();
            }
        }

        [Authorize]
        [FunctionName("StudentFunction_Update")]
        [OpenApiOperation(operationId: "StudentFunction_Update", tags: new[] { StudentsFunctionTag })]
        [OpenApiRequestBody(Constants.ContentTypeJson, typeof(UpdateStudentModel), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: Constants.ContentTypeJson, bodyType: typeof(WrappedResponse<StudentDto>))]
        [OpenApiSecurity(Constants.OpenApiBearer, SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = Constants.OpenApJwt)]
        public async Task<IActionResult> Update([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "students")] HttpRequest req)
        {
            try
            {
                // get model object
                var model = await req.GetJsonBody<UpdateStudentModel>();

                // validate model
                var validationResult = await Helpers.ValidateAsync<UpdateStudentModel, UpdateStudentModel.ValidatorClass>(model);
                if (!validationResult.IsValid)
                {
                    return validationResult.ToBadRequest();
                }

                // run handler
                var response = await _studentService.Update(validationResult.Value!);
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

