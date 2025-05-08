using System.Collections.Generic;
using MovieApp.Models.Search;

namespace MovieApp.Models
{
    public class PopularActorsViewModel
    {
        public List<PersonResult> PopularActors { get; set; } = new List<PersonResult>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalResults { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
