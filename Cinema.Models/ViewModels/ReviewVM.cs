using System.ComponentModel.DataAnnotations;

namespace Cinema.Models.ViewModels
{
    public class ReviewVM
    {
        [Required]
        public int MovieId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars")]
        public int Rating { get; set; }

        public string? Comment { get; set; }
    }
}