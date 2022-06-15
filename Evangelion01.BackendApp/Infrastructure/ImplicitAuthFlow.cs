using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.OpenApi.Models;
using System;

namespace Evangelion01.BackendApp.Infrastructure
{
    public class ImplicitAuthFlow : OpenApiOAuthSecurityFlows
    {
        private const string PolicyName = "B2C_1_signupsignin";
        private const string AuthorisationUrl =
            "https://evangelion01msapac.b2clogin.com/evangelion01msapac.onmicrosoft.com/{0}/oauth2/v2.0/authorize";
        private const string RefreshUrl =
            "https://evangelion01msapac.b2clogin.com/evangelion01msapac.onmicrosoft.com/{0}/oauth2/v2.0/token";

        public ImplicitAuthFlow()
        {
            Implicit = new OpenApiOAuthFlow()
            {
                AuthorizationUrl = new Uri(string.Format(AuthorisationUrl, PolicyName)),
                TokenUrl = new Uri(string.Format(RefreshUrl, PolicyName)),
                
                Scopes = {
                    { "openid", "Default scope defined in the app" }
                }
            };
        }
    }
}