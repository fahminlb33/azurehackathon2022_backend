using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using System;

namespace Evangelion01.BackendApp.Infrastructure
{
    public class OpenApiConfigurationOptions : DefaultOpenApiConfigurationOptions
    {
        public override bool ForceHttps { get; set; } = true;
        public override OpenApiVersionType OpenApiVersion { get; set; } = OpenApiVersionType.V3;
        public override OpenApiInfo Info { get; set; } = new OpenApiInfo
        {
            Version = "1.0.0",
            Title = "Evangelion01",
            Description = "Evangelion01 API Documentation",
            License = new OpenApiLicense
            {
                Name = "MIT",
                Url = new Uri("http://opensource.org/licenses/MIT"),
            }
        };
    }
}
