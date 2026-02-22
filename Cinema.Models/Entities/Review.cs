using System.ComponentModel.DataAnnotations;

namespace Cinema.Models.Entities
{
    public class Review : BaseEntity
    {
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }

        [Required]
        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }
        // Navigation to ApplicationUser will be added after Identity setup
    }
}