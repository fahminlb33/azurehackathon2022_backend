using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Evangelion01.Contracts.Models
{
    public class Grade
    {
        public const string Container = "grades";

        [JsonProperty("id")]
        public string GradeId { get; set; }
        public string StudentId { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public GradeSubject Subject { get; set; }
        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public IList<GradeCategory> Categories { get; set; }
        public int Semester { get; set; }
        public double Score { get; set; }
    }
}
