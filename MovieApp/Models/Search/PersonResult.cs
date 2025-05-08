using System.Text.Json.Serialization;

namespace MovieApp.Models.Search
{
    public class PersonResult
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("profile_path")]
        public string? ProfilePath { get; set; }

        [JsonPropertyName("known_for_department")]
        public string? KnownForDepartment { get; set; }
        public string? FullProfilePath => !string.IsNullOrEmpty(ProfilePath)
                                        ? $"https://image.tmdb.org/t/p/w185{ProfilePath}"
                                        : null; 
    }
}