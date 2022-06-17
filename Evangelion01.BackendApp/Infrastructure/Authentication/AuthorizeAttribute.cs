using Evangelion01.BackendApp.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Threading;
using System.Threading.Tasks;

namespace Evangelion01.BackendApp.Infrastructure.Authentication
{
    public class AuthorizeAttribute : FunctionInvocationFilterAttribute
    {
        public override async Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
        {
            var request = (HttpRequest)executingContext.Arguments["req"];
            var bearerToken = request.GetBearerToken();
            var user = await AzureAdHandler.ValidateAccessTokenAsync(bearerToken, executingContext.Logger);

            if (user != null)
            {
                request.HttpContext.User = user;
            }
            else
            {
                request.HttpContext.Response.StatusCode = 401;
                await request.HttpContext.Response.Body.FlushAsync();
                request.HttpContext.Response.Body.Close();
            }
        }
    }
}
