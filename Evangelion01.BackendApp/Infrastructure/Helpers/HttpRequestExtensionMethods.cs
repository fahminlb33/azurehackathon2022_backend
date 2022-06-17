using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Evangelion01.BackendApp.Infrastructure.Helpers
{
    public static class ExtensionMethods
    {
        public static string GetUserId(this HttpRequest req)
        {
            return req.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public static string? GetBearerToken(this HttpRequest req)
        {
            var authorizationHeader = req.Headers?["Authorization"];
            var parts = authorizationHeader?.ToString().Split(null) ?? Array.Empty<string>();
            return parts.Length == 2 && parts[0].Equals("Bearer") ? parts[1] : null;
        }

        public static async Task<T?> GetJsonBody<T>(this HttpRequest request)
        {
            var requestBody = await request.ReadAsStringAsync();
            if (requestBody == null) return default;

            return JsonConvert.DeserializeObject<T>(requestBody);
        }

        public static async Task<T?> GetJsonQuery<T>(this HttpRequest request)
        {
            var qsSerialized = JsonConvert.SerializeObject(request.GetQueryParameterDictionary());
            var model = JsonConvert.DeserializeObject<T>(qsSerialized);
            return model;
        }

        public static BadRequestObjectResult ToBadRequest<T>(this ValidatableRequest<T> request)
        {
            return new BadRequestObjectResult(request.Errors.Select(e => new
            {
                Field = e.PropertyName,
                Error = e.ErrorMessage
            }));
        }
    }
}
