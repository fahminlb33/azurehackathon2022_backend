using FluentValidation;
using FluentValidation.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Evangelion01.BackendApp.Infrastructure.Helpers
{
    public static class Helpers
    {
        public static async Task<ValidatableRequest<T>> ValidateAsync<T, V>(T? model) where V : AbstractValidator<T>, new()
        {
            if (model == null)
            {
                return new ValidatableRequest<T>
                {
                    Value = model,
                    IsValid = false,
                    Errors = new List<ValidationFailure>
                    {
                        new ValidationFailure("Body", "Body is null")
                    }
                };
            }

            var validator = new V();
            var validationResult = await validator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                return new ValidatableRequest<T>
                {
                    Value = model,
                    IsValid = false,
                    Errors = validationResult.Errors
                };
            }

            return new ValidatableRequest<T>
            {
                Value = model,
                IsValid = true
            };
        }
    }
}
