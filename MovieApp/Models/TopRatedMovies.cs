using System.Collections.Generic;
using MovieApp.Models; 

namespace MovieApp.Models
{
    public class TopRatedMovieItem
    {
        public int Rank { get; set; }

        public MovieResult Movie { get; set; } = null!;

    }
    public class TopRatedMoviesViewModel
    {
        public List<TopRatedMovieItem> MoviesWithRank { get; set; } = new List<TopRatedMovieItem>();

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalResults { get; set; } 

        public bool HasPreviousPage => CurrentPage > 1;

        public bool HasNextPage => CurrentPage < TotalPages;
    }
}