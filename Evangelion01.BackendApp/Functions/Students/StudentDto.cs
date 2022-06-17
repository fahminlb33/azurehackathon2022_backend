using Evangelion01.Contracts.Models;

namespace Evangelion01.BackendApp.Functions.Students
{
    public class StudentDto
    {
        public string StudentId { get; set; }
        public string Name { get; set; }
        public StudentGroup Group { get; set; }
    }
}