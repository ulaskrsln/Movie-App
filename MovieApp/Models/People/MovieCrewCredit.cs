using System.Text.Json.Serialization;
using System;

namespace MovieApp.Models.People
{
    public class MovieCrewCredit
    {
        [JsonPropertyName("id")]
        public int Id { get; set; } 

        [JsonPropertyName("title")]
        public string? Title { get; set; } 

        [JsonPropertyName("job")]
        public string? Job { get; set; } 

        [JsonPropertyName("department")]
        public string? Department { get; set; } 

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("release_date")]
        public string? ReleaseDate { get; set; }

        public string? FullPosterPath => !string.IsNullOrEmpty(PosterPath)
                                        ? $"https://image.tmdb.org/t/p/w185{PosterPath}"
                                        : null;

        public string ReleaseYear => !string.IsNullOrEmpty(ReleaseDate) && DateTime.TryParse(ReleaseDate, out var date)
                                        ? date.Year.ToString()
                                        : "N/A";
    }
}