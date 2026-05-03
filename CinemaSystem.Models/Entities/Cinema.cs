using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace CinemaSystem.Models.Entities
{
    public class Cinema : BaseEntity
    {
        public Cinema()
        {
            Halls = new List<CinemaHall>();
        }

        [Required(ErrorMessage = "Cinema name is required.")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [MaxLength(50)]
        public string City { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [MaxLength(200)]
        public string Address { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [ValidateNever]
        [MaxLength(255)]
        public string? Logo { get; set; }

        public virtual List<CinemaHall> Halls { get; set; }
    }
}