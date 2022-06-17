using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Evangelion01.Contracts.Models
{
    public class Student
    {
        public const string Container = "students";

        [JsonProperty("id")]
        public string StudentId { get; set; }
        public string Name { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public StudentGroup Group { get; set; }
    }
}
