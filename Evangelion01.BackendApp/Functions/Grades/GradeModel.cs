using Evangelion01.Contracts.Models;
using FluentValidation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Evangelion01.BackendApp.Functions.Grades
{
    public class GetGradeModel
    {
        public string GradeId { get; set; }
        
        public class ValidatorClass : AbstractValidator<GetGradeModel>
        {
            public ValidatorClass()
            {
                RuleFor(x => x.GradeId).NotEmpty().Length(36);
            }
        }
    }

    public class GetAllGradeModel
    {
        public string? UserId { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public GradeSubject? Subject { get; set; }
        public int? Semester { get; set; }
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;

        public class ValidatorClass : AbstractValidator<GetAllGradeModel>
        {
            public ValidatorClass()
            {
                RuleFor(x => x.UserId).Length(36).When(x => !string.IsNullOrEmpty(x.UserId));
                RuleFor(x => x.Subject).IsInEnum().When(x => x.Subject.HasValue);
                RuleFor(x => x.Semester).GreaterThan(0).LessThanOrEqualTo(5).When(x => x.Semester.HasValue);

                RuleFor(x => x.Page).GreaterThan(0);
                RuleFor(x => x.Limit).GreaterThanOrEqualTo(1).LessThanOrEqualTo(100);
            }
        }
    }

    public class AddGradeModel
    {
        public string UserId { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public GradeSubject Subject { get; set; }
        public int Semester { get; set; }
        public int Value { get; set; }

        public class ValidatorClass : AbstractValidator<AddGradeModel>
        {
            public ValidatorClass()
            {
                RuleFor(x => x.UserId).NotEmpty().Length(36);
                RuleFor(x => x.Subject).IsInEnum();
                RuleFor(x => x.Semester).GreaterThan(0).LessThanOrEqualTo(5);
                RuleFor(x => x.Value).GreaterThan(0).LessThanOrEqualTo(100);
            }
        }
    }

    public class DeleteGradeModel
    {
        public string GradeId { get; set; }

        public class ValidatorClass : AbstractValidator<DeleteGradeModel>
        {
            public ValidatorClass()
            {
                RuleFor(x => x.GradeId).NotEmpty().Length(36);
            }
        }
    }

    public class UpdateGradeModel
    {
        public string GradeId { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public GradeSubject? Subject { get; set; }
        public int? Semester { get; set; }
        public int? Score { get; set; }

        public class ValidatorClass : AbstractValidator<UpdateGradeModel>
        {
            public ValidatorClass()
            {
                RuleFor(x => x.GradeId).NotEmpty().Length(36);
                RuleFor(x => x.Subject).IsInEnum();
                RuleFor(x => x.Semester).GreaterThan(0).LessThanOrEqualTo(5);
                RuleFor(x => x.Score).GreaterThan(0).LessThanOrEqualTo(100);
            }
        }
    }
}
