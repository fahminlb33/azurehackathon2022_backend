using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Evangelion01.BackendApp.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Evangelion01.Functions.Grades
{
    public class GradeFunction
    {
        private readonly ILogger<GradeFunction> _logger;

        public GradeFunction(ILogger<GradeFunction> log)
        {
            _logger = log;
        }

        [FunctionName("GradesFunction_Add")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var studentId = req.HttpContext.GetStudentId();
            var principalSerialized = JsonConvert.SerializeObject(req.HttpContext.User);
            _logger.LogDebug(principalSerialized);


            return new OkResult();
        }

        [FunctionName("GradesFunction_GetAll")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> GetAll(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var principalSerialized = JsonConvert.SerializeObject(req.HttpContext.User);
            _logger.LogDebug(principalSerialized);

            var studentId = req.HttpContext.GetStudentId();


            return new OkResult();
        }
    }
}

