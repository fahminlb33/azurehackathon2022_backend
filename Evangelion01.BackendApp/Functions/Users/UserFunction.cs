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
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Evangelion01.Functions.Users
{
    public class UserFunction
    {
        const string OidcMetadataUrl = "https://evangelion01msapac.b2clogin.com/evangelion01msapac.onmicrosoft.com/B2C_1_signupsignin/v2.0/.well-known/openid-configuration";
        private readonly ILogger<UserFunction> _logger;

        public UserFunction(ILogger<UserFunction> log)
        {
            _logger = log;
        }

        [FunctionName("UsersFunction")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        //[OpenApiSecurity("implicit_auth", SecuritySchemeType.OAuth2, Flows = typeof(ImplicitAuthFlow))]
        //[OpenApiSecurity("apikeyheader_auth", SecuritySchemeType.ApiKey, In = OpenApiSecurityLocationType.Header, Name = "x-functions-key")]
        [OpenApiSecurity("bearer_auth", SecuritySchemeType.Http, In = OpenApiSecurityLocationType.Header, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        //[OpenApiSecurity("oidc", SecuritySchemeType.OpenIdConnect, OpenIdConnectScopes = "openid", OpenIdConnectUrl = OidcMetadataUrl)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.User, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            foreach (var claim in req.HttpContext.User.Claims)
            {
                Debug.Print(claim.Type + " - " + claim.Value);
            }

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}

