using Evangelion01.BackendApp.Functions.Grades;
using Evangelion01.BackendApp.Functions.Predictions;
using Evangelion01.BackendApp.Functions.Students;
using Evangelion01.BackendApp.Infrastructure;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;
using System.IO;

[assembly: FunctionsStartup(typeof(Evangelion01.BackendApp.Startup))]

namespace Evangelion01.BackendApp
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            var context = builder.GetContext();

            builder.ConfigurationBuilder
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, "appsettings.json"), optional: true, reloadOnChange: false)
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"appsettings.{context.EnvironmentName}.json"), optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;
            var config = builder.GetContext().Configuration;

            // register ASP.NET services
            services.AddLogging();
            services.AddHttpClient();
            services.AddAutoMapper(typeof(GradeMapperProfile));

            // register function services
            services.AddSingleton<IGradeService, GradeService>();
            services.AddSingleton<IStudentService, StudentService>();
            services.AddSingleton<IPredictionService, PredictionService>();

            // register infrastructure components
            services.AddSingleton((provider) =>
            {
                return new JsonSerializer()
                {
                    DateParseHandling = DateParseHandling.DateTimeOffset,
                    DateTimeZoneHandling = DateTimeZoneHandling.Local,
                    Converters =
                    {
                        new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
                    }
                };
            });

            // register cosmos db
            services.AddSingleton((factory) =>
            {
                var cosmosClientBuilder = new CosmosClientBuilder(EnvironmentConfig.DatabaseConnectionString);
                return cosmosClientBuilder
                    .WithConnectionModeDirect()
                    .Build();
            });
        }
    }
}
