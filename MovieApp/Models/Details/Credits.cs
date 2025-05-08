using System.Text.Json.Serialization;

namespace MovieApp.Models.Details
{
    public class Credits
    {
        [JsonPropertyName("cast")]
        public List<Actor> Cast { get; set; } = new List<Actor>();

        [JsonPropertyName("crew")]
        public List<CrewMember> Crew { get; set; } = new List<CrewMember>();
    }
}
