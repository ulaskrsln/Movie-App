using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System;


namespace MovieApp.Models.Details
{
    public class MovieDetail
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("overview")]
        public string? Overview { get; set; } 

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string? BackdropPath { get; set; } 

        [JsonPropertyName("release_date")]
        public string? ReleaseDate { get; set; }

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [JsonPropertyName("vote_count")]
        public int VoteCount { get; set; }

        [JsonPropertyName("runtime")]
        public int? Runtime { get; set; } 

        [JsonPropertyName("genres")]
        public List<Genre> Genres { get; set; } = new List<Genre>();

        [JsonPropertyName("credits")]
        public Credits? Credits { get; set; }


        public string ReleaseYear => !string.IsNullOrEmpty(ReleaseDate) && DateTime.TryParse(ReleaseDate, out var date)
                                        ? date.Year.ToString()
                                        : "Bilinmiyor";

        public string FormattedRuntime
        {
            get
            {
                if (Runtime.HasValue && Runtime.Value > 0)
                {
                    var timeSpan = TimeSpan.FromMinutes(Runtime.Value);
                    return $"{(int)timeSpan.TotalHours} sa {timeSpan.Minutes} dk";
                }
                return "Bilinmiyor";
            }
        }

        public string DirectorName
        {
            get
            {
                var director = Credits?.Crew?.FirstOrDefault(c => c.Job == "Director");
                return director?.Name ?? "Bilinmiyor";
            }
        }

        public int? DirectorId 
        {
            get
            {
                if (this.Credits?.Crew != null)
                {
                    var director = this.Credits.Crew.FirstOrDefault(c => c.Job == "Director");
                    return director?.Id;
                }
                return null;
            }
        }
        // --- ---

        public List<Actor> CastMembers => Credits?.Cast ?? new List<Actor>();

        public List<Actor> TopCastMembers(int count = 10) => Credits?.Cast?.Take(count).ToList() ?? new List<Actor>();

        public string FormattedVoteAverage => VoteAverage.ToString("0.0");

        public string FullPosterPath => !string.IsNullOrEmpty(PosterPath)
                                        ? $"https://image.tmdb.org/t/p/w500{PosterPath}"
                                        : null; 

        public string FullBackdropPath => !string.IsNullOrEmpty(BackdropPath)
                                        ? $"https://image.tmdb.org/t/p/w1280{BackdropPath}"
                                        : null; 
    }
}