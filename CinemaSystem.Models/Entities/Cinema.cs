using System.ComponentModel.DataAnnotations;

namespace CinemaSystem.Models.Entities
{
    public class Cinema : BaseEntity
    {
        public Cinema()
        {
            Halls = new List<CinemaHall>();
        }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        public string City { get; set; }

        [Required]
        public string Address { get; set; }

        public string? Description { get; set; }

        public string? Logo { get; set; }

        public virtual List<CinemaHall> Halls { get; set; }
    }
}