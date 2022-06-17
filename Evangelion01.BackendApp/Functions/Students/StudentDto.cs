using Evangelion01.Contracts.Models;
using System.Collections.Generic;

namespace Evangelion01.BackendApp.Functions.Students
{
    public class StudentDto
    {
        public string StudentId { get; set; }
        public string Name { get; set; }
        public StudentGroup Group { get; set; }
    }

    public record StudentListDto
    {
        public int TotalData { get; set; }
        public int TotalPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalDataOnPage { get; set; }
        public IList<StudentDto> Records { get; set; }
    }
}