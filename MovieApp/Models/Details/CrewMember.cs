using System.Text.Json.Serialization;

namespace MovieApp.Models.Details
{
    public class CrewMember
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("job")]
        public string Job { get; set; } = string.Empty; //örn; "Director"
    }
}
