using FluentValidation;

namespace Evangelion01.BackendApp.Functions.Predictions
{
    public class PredictionModel
    {
        public string UserId { get; set; }

        public class ValidatorClass : AbstractValidator<PredictionModel>
        {
            public ValidatorClass()
            {
                RuleFor(x => x.UserId).NotEmpty().Length(36);
            }
        }
    }
}
