using FluentValidation;

namespace Evangelion01.BackendApp.Functions.Predictions
{
    public class PredictionModel
    {
        public string StudentId { get; set; }

        public class ValidatorClass : AbstractValidator<PredictionModel>
        {
            public ValidatorClass()
            {
                RuleFor(x => x.StudentId).NotEmpty().Length(36);
            }
        }
    }
}
