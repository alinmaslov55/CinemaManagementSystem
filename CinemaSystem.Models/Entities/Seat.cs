using System.ComponentModel.DataAnnotations;
using CinemaSystem.Models.Data.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CinemaSystem.Models.Entities
{
    public class Seat : BaseEntity
    {
        [Required]
        public string Row { get; set; }

        [Required]
        public int Column { get; set; }

        [Required]
        public SeatType SeatType { get; set; }

        [Required]
        public bool IsAccessible { get; set; }

        [Required]
        public int CinemaHallId { get; set; }
        [ValidateNever]
        public virtual CinemaHall CinemaHall { get; set; }
    }
}