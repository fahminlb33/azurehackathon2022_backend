using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Evangelion01.BackendApp.Infrastructure.Authentication
{
    internal class AzureAdHandler
    {
        public static async Task<ClaimsPrincipal?> ValidateAccessTokenAsync(string accessToken, ILogger log)
        {
            var oidcRetriever = new OpenIdConnectConfigurationRetriever();
            var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(EnvironmentConfig.OidcWellKnownUri, oidcRetriever);

            var config = await configManager.GetConfigurationAsync();
            var tokenValidator = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidAudiences = new[] { EnvironmentConfig.ApplicationId },
                ValidIssuers = new[]
                {
                    $"https://evangelion01msapac.b2clogin.com/{EnvironmentConfig.TenantId}/v2.0/"
                },
                ValidateIssuer = true,
                ValidateAudience = true,
                IssuerSigningKeys = config.SigningKeys
            };

            try
            {
                var claimsPrincipal = tokenValidator.ValidateToken(accessToken, validationParameters, out var securityToken);
                return claimsPrincipal;
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return null;
            }
        }

        public static bool UserIsInAppRole(ClaimsPrincipal user, string[] validAppRoles)
        {
            return user.Claims
                .Where(e => e.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                .Select(e => e.Value)
                .Intersect(validAppRoles)
                .Any();
        }
    }
}
