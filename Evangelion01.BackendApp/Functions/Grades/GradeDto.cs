using Evangelion01.Contracts.Models;

namespace Evangelion01.BackendApp.Functions.Grades
{
    public record GradeDto
    {
        public string GradeId { get; set; }
        public string StudentId { get; set; }
        public GradeSubject Subject { get; set; }
        public int Semester { get; set; }
        public int Value { get; set; }
    }
}
