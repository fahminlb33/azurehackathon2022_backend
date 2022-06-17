using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Evangelion01.Contracts.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StudentGroup
    {
        MIPA,
        IPS,
        Bahasa
    }
}
