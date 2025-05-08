using System.Collections.Generic;
using MovieApp.Models.Details;

namespace MovieApp.Models
{
    public class MoviesByGenreViewModel
    {
        public Genre GenreInfo { get; set; } = null!; 

        public List<MovieResult> Movies { get; set; } = new List<MovieResult>();

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalResults { get; set; } 
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}