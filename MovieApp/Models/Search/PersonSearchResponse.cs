using System.Text.Json.Serialization;

namespace MovieApp.Models.Search
{
    public class PersonSearchResponse
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("results")]
        public List<PersonResult> Results { get; set; } = new List<PersonResult>();

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("total_results")]
        public int TotalResults { get; set; }
    }
}
