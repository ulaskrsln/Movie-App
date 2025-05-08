using System.Text.Json.Serialization;
using System;

namespace MovieApp.Models.People
{
    public class MovieCastCredit
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("character")]
        public string? Character { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("release_date")]
        public string? ReleaseDate { get; set; }

        public string? FullPosterPath => !string.IsNullOrEmpty(PosterPath) ? $"https://image.tmdb.org/t/p/w185{PosterPath}" : null;
        public string ReleaseYear => !string.IsNullOrEmpty(ReleaseDate) && DateTime.TryParse(ReleaseDate, out var date) ? date.Year.ToString() : "N/A";
    }
}