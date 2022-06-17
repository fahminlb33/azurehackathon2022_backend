using Evangelion01.Contracts.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class GradeCsv
{
    public const string Container = "grades";

    [JsonProperty("id")]
    public string GradeId { get; set; }
    public string StudentId { get; set; }
    [JsonConverter(typeof(StringEnumConverter))]
    public GradeSubject Subject { get; set; }
    public int Semester { get; set; }
    public double Score { get; set; }
}