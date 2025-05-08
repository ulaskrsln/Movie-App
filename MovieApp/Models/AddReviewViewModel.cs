using System.ComponentModel.DataAnnotations;

namespace MovieApp.Models
{
    public class AddReviewViewModel
    {
        [Required]
        public int MovieId { get; set; }
        [Required(ErrorMessage = "Lütfen bir rumuz girin.")]
        [Display(Name = "Rumuzunuz")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Rumuz 3 ile 50 karakter arasında olmalıdır.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lütfen filme 1-10 arası bir puan verin.")]
        [Range(1, 10, ErrorMessage = "Puan 1 ile 10 arasında olmalıdır.")]
        [Display(Name = "Puanınız (1-10)")]
        public int Rating { get; set; }

        [Display(Name = "Yorumunuz (İsteğe Bağlı)")]
        [DataType(DataType.MultilineText)] 
        [StringLength(1000, ErrorMessage = "Yorum en fazla 1000 karakter olabilir.")]
        public string? CommentText { get; set; }
    }
}
