using System.Text.Json.Serialization;

namespace MovieApp.Models
{
    public class MovieResult
    {
        [JsonPropertyName("id")] 
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("overview")]
        public string Overview { get; set; } = string.Empty;

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; } 

        [JsonPropertyName("release_date")]
        public string? ReleaseDate { get; set; }

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [JsonPropertyName("popularity")]
        public double Popularity { get; set; }

        [JsonPropertyName("vote_count")]
        public int VoteCount { get; set; }

        public string FullPosterPath => !string.IsNullOrEmpty(PosterPath)
                                        ? $"https://image.tmdb.org/t/p/w500{PosterPath}"
                                        : "/images/placeholder-poster.png";

        public string ReleaseYear => !string.IsNullOrEmpty(ReleaseDate) && DateTime.TryParse(ReleaseDate, out var date)
                                        ? date.Year.ToString()
                                        : "N/A";

    }
}