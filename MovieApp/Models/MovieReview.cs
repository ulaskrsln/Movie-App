using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Models
{
    public class MovieReview
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MovieId { get; set; }

        [Required(ErrorMessage = "Lütfen bir rumuz girin.")]
        [StringLength(50, ErrorMessage = "Rumuz en fazla 50 karakter olabilir.")]

        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lütfen filme 1-10 arası bir puan verin.")]
        [Range(1, 10, ErrorMessage = "Puan 1 ile 10 arasında olmalıdır.")]
        public int Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Yorum en fazla 1000 karakter olabilir.")]
        public string? CommentText { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}

