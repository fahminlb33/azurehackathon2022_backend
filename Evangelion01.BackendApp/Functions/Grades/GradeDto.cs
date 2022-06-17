using Evangelion01.Contracts.Models;
using System.Collections.Generic;

namespace Evangelion01.BackendApp.Functions.Grades
{
    public record GradeDto
    {
        public string GradeId { get; set; }
        public string StudentId { get; set; }
        public GradeSubject Subject { get; set; }
        public int Semester { get; set; }
        public int Score { get; set; }
    }

    public record GradeListDto
    {
        public int TotalData { get; set; }
        public int TotalPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalDataOnPage { get; set; }
        public IList<GradeDto> Records { get; set; }
    }
}