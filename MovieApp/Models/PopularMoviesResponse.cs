using System.Text.Json.Serialization; 

namespace MovieApp.Models
{
    public class PopularMoviesResponse
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("results")]
        public List<MovieResult> Results { get; set; } = new List<MovieResult>();

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("total_results")]
        public int TotalResults { get; set; }
    }
}