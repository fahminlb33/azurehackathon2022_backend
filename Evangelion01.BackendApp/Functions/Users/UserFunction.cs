using System.Threading.Tasks;
using Evangelion01.BackendApp.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Evangelion01.Functions.Users
{
    public class UserFunction
    {
        private readonly ILogger<UserFunction> _logger;

        public UserFunction(ILogger<UserFunction> log)
        {
            _logger = log;
        }

        [FunctionName("UsersFunction_LoginRedirect")]
        [OpenApiOperation(operationId: "LoginRedirect", tags: new[] { "Users API" })]
        public async Task<IActionResult> LoginRedirect([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/launch_login")] HttpRequest req)
        {
            _logger.LogInformation("Redirecting to login page...");

            await Task.CompletedTask;
            return new RedirectResult(EnvironmentConfig.LoginUri);
        }
    }
}

