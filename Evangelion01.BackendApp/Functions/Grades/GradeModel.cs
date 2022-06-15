using Evangelion01.Contracts.Models;
using FluentValidation;

namespace Evangelion01.BackendApp.Functions.Grades
{
    public class AddGradeModel
    {
        public string StudentId { get; set; }
        public GradeSubject Subject { get; set; }
        public int Semester { get; set; }
        public int Value { get; set; }
    }

    public class AddGradeModelValidator : AbstractValidator<AddGradeModel>
    {
        public AddGradeModelValidator()
        {
            RuleFor(x => x.Subject).IsInEnum();
            RuleFor(x => x.Semester).GreaterThan(0).LessThanOrEqualTo(5);
            RuleFor(x => x.Value).GreaterThan(0).LessThanOrEqualTo(100);
        }
    }
}
