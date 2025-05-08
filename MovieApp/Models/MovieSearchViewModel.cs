using System.Collections.Generic;
using MovieApp.Models.Details;
using MovieApp.Models.Search;
namespace MovieApp.Models
{
    public class MovieSearchViewModel
    {
        public string Query { get; set; } = string.Empty;

        public List<MovieResult> MovieResults { get; set; } = new List<MovieResult>();
        public int MovieCurrentPage { get; set; } = 1;
        public int MovieTotalPages { get; set; }
        public int MovieTotalResults { get; set; }
        public bool HasPreviousMoviePage => MovieCurrentPage > 1;
        public bool HasNextMoviePage => MovieCurrentPage < MovieTotalPages;


        public List<PersonResult> PersonResults { get; set; } = new List<PersonResult>();
        public int PersonCurrentPage { get; set; } = 1;
        public int PersonTotalPages { get; set; }
        public int PersonTotalResults { get; set; }
    }
}