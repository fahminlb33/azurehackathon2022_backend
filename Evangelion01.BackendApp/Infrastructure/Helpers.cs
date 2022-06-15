using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evangelion01.BackendApp.Infrastructure
{
    public static class Helpers
    {
        public static async Task<T> GetJsonBody<T>(this HttpRequest request)
        {
            var requestBody = await request.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(requestBody);
        }

        public static async Task<ValidatableRequest<T>> GetJsonBody<T, V>(this HttpRequest request) where V : AbstractValidator<T>, new()
        {
            var requestObject = await request.GetJsonBody<T>();
            var validator = new V();
            var validationResult = validator.Validate(requestObject);

            if (!validationResult.IsValid)
            {
                return new ValidatableRequest<T>
                {
                    Value = requestObject,
                    IsValid = false,
                    Errors = validationResult.Errors
                };
            }
            
            return new ValidatableRequest<T>
            {
                Value = requestObject,
                IsValid = true
            };
        }

        public static BadRequestObjectResult ToBadRequest<T>(this ValidatableRequest<T> request)
        {
            return new BadRequestObjectResult(request.Errors.Select(e => new {
                Field = e.PropertyName,
                Error = e.ErrorMessage
            }));
        }
    }
}
