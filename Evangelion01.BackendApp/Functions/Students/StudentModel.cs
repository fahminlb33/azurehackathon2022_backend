using Evangelion01.Contracts.Models;
using FluentValidation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Evangelion01.BackendApp.Functions.Students
{
    public class GetStudentModel
    {
        public string StudentId { get; set; }

        public class ValidatorClass : AbstractValidator<GetStudentModel>
        {
            public ValidatorClass()
            {
                RuleFor(x => x.StudentId).NotEmpty().Length(36);
            }
        }
    }

    public class GetAllStudentModel
    {
        public string? Keyword { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public StudentGroup? Group { get; set; }
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;

        public class ValidatorClass : AbstractValidator<GetAllStudentModel>
        {
            public ValidatorClass()
            {
                RuleFor(x => x.Keyword).MinimumLength(3).When(x => !string.IsNullOrEmpty(x.Keyword));
                RuleFor(x => x.Group).IsInEnum().When(x => x.Group.HasValue);

                RuleFor(x => x.Page).GreaterThan(0);
                RuleFor(x => x.Limit).GreaterThanOrEqualTo(1).LessThanOrEqualTo(100);
            }
        }
    }

    public class AddStudentModel
    {
        public string Name { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public StudentGroup Group { get; set; }

        public class ValidatorClass : AbstractValidator<AddStudentModel>
        {
            public ValidatorClass()
            {
                RuleFor(x => x.Name).NotEmpty().MinimumLength(5);
                RuleFor(x => x.Group).IsInEnum();
            }
        }
    }

    public class DeleteStudentModel
    {
        public string StudentId { get; set; }

        public class ValidatorClass : AbstractValidator<DeleteStudentModel>
        {
            public ValidatorClass()
            {
                RuleFor(x => x.StudentId).NotEmpty().Length(36);
            }
        }
    }

    public class UpdateStudentModel
    {
        public string StudentId { get; set; }
        public string Name { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public StudentGroup Group { get; set; }

        public class ValidatorClass : AbstractValidator<UpdateStudentModel>
        {
            public ValidatorClass()
            {
                RuleFor(x => x.StudentId).NotEmpty().Length(36);
                RuleFor(x => x.Name).NotEmpty().MinimumLength(5);
                RuleFor(x => x.Group).IsInEnum();
            }
        }
    }
}
