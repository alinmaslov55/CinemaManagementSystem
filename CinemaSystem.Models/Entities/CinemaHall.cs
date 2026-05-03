using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CinemaSystem.Models.Data.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CinemaSystem.Models.Entities
{
    public class CinemaHall : BaseEntity
    {
        public CinemaHall()
        {
            Seats = new List<Seat>();
            Showtimes = new List<Showtime>();
        }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Total seats must be at least 1.")]
        public int TotalSeats { get; set; }

        [Required(ErrorMessage = "Please select a Hall Type.")]
        [Display(Name = "Hall Type")]
        public HallType HallType { get; set; }

        [Required(ErrorMessage = "Please select a cinema location.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid cinema.")]
        [Display(Name = "Cinema")]
        public int CinemaId { get; set; }

        [ForeignKey("CinemaId")]
        [ValidateNever]
        public virtual Cinema Cinema { get; set; }

        [ValidateNever]
        public virtual List<Seat> Seats { get; set; }

        [ValidateNever]
        public virtual List<Showtime> Showtimes { get; set; }
        [ValidateNever]
        public virtual ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
    }
}