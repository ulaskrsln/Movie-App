using System.Collections.Generic;

namespace MovieApp.Models 
{
    public class HomeIndexViewModel
    {
        public List<MovieResult> Movies { get; set; } = new List<MovieResult>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalResults { get; set; } 

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}