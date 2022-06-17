using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Evangelion01.Contracts.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GradeCategory
    {
        Stem,
        Arts,
        Literature,
        Business,
        Social,
        Others
    }
}
