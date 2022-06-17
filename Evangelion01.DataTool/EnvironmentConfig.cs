using Newtonsoft.Json.Linq;

namespace Evangelion01.DataTool
{
    // This helper class is to provide app config from environment variable
    // because dependency injection currently not supported in function filters
    public class EnvironmentConfig
    {
        // Azure Cosmos DB
        public static string DatabaseName { get; }
        public static string DatabaseConnectionString { get; }

        // Azure AD B2C
        public static string TenantId { get; }
        public static string ApplicationId { get; }
        public static string PolicyName { get; }
        public static string DomainName { get; }
        public static string B2cDomainName { get; }
        public static string OidcWellKnownUri { get; }
        public static string RedirectUri { get; }
        public static string LoginUri { get; }

        // Prediction Services
        public static string PredictionUri { get; }

        static EnvironmentConfig()
        {
            // load from appsettings.json file
            JToken? root = null;
            if (File.Exists("appsettings.json"))
            {
                root = JToken.Parse(File.ReadAllText("appsettings.json"));
            }

            // load configs
            DatabaseName = GetEnv(root, nameof(DatabaseName));
            DatabaseConnectionString = GetEnv(root, nameof(DatabaseConnectionString));

            TenantId = GetEnv(root, nameof(TenantId));
            ApplicationId = GetEnv(root, nameof(ApplicationId));
            PolicyName = GetEnv(root, nameof(PolicyName));
            DomainName = GetEnv(root, nameof(DomainName));
            B2cDomainName = GetEnv(root, nameof(B2cDomainName));
            RedirectUri = GetEnv(root, nameof(RedirectUri));

            PredictionUri = GetEnv(root, nameof(PredictionUri));

            OidcWellKnownUri = $"https://{B2cDomainName}/{DomainName}/{PolicyName}/v2.0/.well-known/openid-configuration";
            LoginUri = $"https://{B2cDomainName}/{DomainName}/oauth2/v2.0/authorize?p={PolicyName}&client_id={ApplicationId}&nonce=defaultNonce&redirect_uri={RedirectUri}&scope=openid&response_type=id_token&prompt=login";
        }

        private static string GetEnv(JToken? root, string key, string defaultValue = "")
        {
            string? valueCandidate = null;

            // get config from JSON file if available
            if (root != null)
            {
                valueCandidate = root.Value<string>(key);
            }

            // get value from environment variable if JSON file not available
            if (valueCandidate == null)
            {
                valueCandidate = Environment.GetEnvironmentVariable(key);
            }

            return string.IsNullOrWhiteSpace(valueCandidate) ? defaultValue : valueCandidate;
        }
    }
}
