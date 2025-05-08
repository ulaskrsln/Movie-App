using System.Collections.Generic;   
using System.Text.Json.Serialization; 
using MovieApp.Models.Details;

namespace MovieApp.Models
{
    public class GenreListResponse
    {
        [JsonPropertyName("genres")]
        public List<Genre> Genres { get; set; } = new List<Genre>();
    }
}