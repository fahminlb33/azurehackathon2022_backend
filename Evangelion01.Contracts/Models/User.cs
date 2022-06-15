using Newtonsoft.Json;

namespace Evangelion01.Contracts.Models
{
    public class User
    {
        public const string Container = "users";

        [JsonProperty("id")]
        public string StudentId { get; set; }
        public string Name { get; set; }
        public UserGroup Group { get; set; }
    }
}
