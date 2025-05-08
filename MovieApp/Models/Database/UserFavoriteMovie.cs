using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 
using MovieApp.Models.Identity;

namespace MovieApp.Models.Database 
{
    public class UserFavoriteMovie
    {
        [Required]
        public string ApplicationUserId { get; set; } = null!;

        [Required]
        public int MovieId { get; set; } 

        [Required]
        public DateTime AddedDate { get; set; }

        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; } = null!;
    }
}