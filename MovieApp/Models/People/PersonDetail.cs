using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MovieApp.Models.People 
{
    public class MovieCreditsResponse
    {
        [JsonPropertyName("cast")]
        public List<MovieCastCredit> Cast { get; set; } = new List<MovieCastCredit>();

        [JsonPropertyName("crew")]
        public List<MovieCrewCredit> Crew { get; set; } = new List<MovieCrewCredit>();
    }

    public class PersonDetail 
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("biography")]
        public string? Biography { get; set; }

        [JsonPropertyName("birthday")]
        public string? Birthday { get; set; }

        [JsonPropertyName("deathday")]
        public string? Deathday { get; set; }

        [JsonPropertyName("place_of_birth")]
        public string? PlaceOfBirth { get; set; }

        [JsonPropertyName("profile_path")]
        public string? ProfilePath { get; set; }

        [JsonPropertyName("known_for_department")]
        public string? KnownForDepartment { get; set; }

        [JsonPropertyName("popularity")]
        public double Popularity { get; set; }

        [JsonPropertyName("movie_credits")]
        public MovieCreditsResponse? MovieCredits { get; set; } 


        public string? FullProfilePath => !string.IsNullOrEmpty(ProfilePath) ? $"https://image.tmdb.org/t/p/h632{ProfilePath}" : null;
        public string? FormattedBirthday => DateTime.TryParse(Birthday, out var date) ? date.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("tr-TR")) : null;
        public int? Age
        {
            get
            {
                if (string.IsNullOrEmpty(Birthday) || !DateTime.TryParse(Birthday, out var birthDate)) return null;
                var endDate = string.IsNullOrEmpty(Deathday) || !DateTime.TryParse(Deathday, out var deathDate) ? DateTime.Today : deathDate;
                int age = endDate.Year - birthDate.Year;
                if (birthDate.Date > endDate.AddYears(-age)) age--;
                return age;
            }
        }
        public string AgeString => Age.HasValue ? (Deathday == null ? $"{Age} yaşında" : $"Vefat ettiğinde {Age} yaşındaydı") : "Bilinmiyor";


        public List<MovieCastCredit> SortedMovieCredits 
        {
            get
            {
                return MovieCredits?.Cast
                    ?.Where(c => !string.IsNullOrEmpty(c.ReleaseDate))
                    .OrderByDescending(c => DateTime.TryParse(c.ReleaseDate, out var date) ? date : DateTime.MinValue)
                    .ToList()
                    ?? new List<MovieCastCredit>();
            }
        }

        public List<MovieCrewCredit> DirectedMovies
        {
            get
            {
                return MovieCredits?.Crew
                    ?.Where(c => c.Job == "Director" && !string.IsNullOrEmpty(c.ReleaseDate))
                    .OrderByDescending(c => DateTime.TryParse(c.ReleaseDate, out var date) ? date : DateTime.MinValue)
                    .ToList()
                    ?? new List<MovieCrewCredit>();
            }
        }

    }
}