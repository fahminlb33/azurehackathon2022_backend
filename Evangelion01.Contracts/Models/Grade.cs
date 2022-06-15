using Newtonsoft.Json;

namespace Evangelion01.Contracts.Models
{
    public class Grade
    {
        public const string Container = "grades";

        [JsonProperty("id")]
        public string GradeId { get; set; }
        public string StudentId { get; set; }
        public GradeSubject Subject { get; set; }
        public IList<GradeCategory> Categories { get; set; }
        public int Semester { get; set; }
        public int Value { get; set; }
    }
}
