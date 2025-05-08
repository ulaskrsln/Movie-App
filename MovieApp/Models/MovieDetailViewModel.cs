using System.Collections.Generic;
using MovieApp.Models.Details;
using MovieApp.Models.Database;

namespace MovieApp.Models
{
    public class MovieDetailViewModel
    {
        public MovieDetail Movie { get; set; } = null;
        public List<MovieReview> Reviews { get; set; } = new List<MovieReview>();
        public AddReviewViewModel NewReview { get; set; } = new AddReviewViewModel();
        public bool IsFavorited { get; set; } = false;
    }
}
