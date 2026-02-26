using CinemaSystem.Models.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CinemaSystem.Models.ViewModels
{
    public class CinemaHallVM
    {
        public CinemaHall CinemaHall { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> CinemaList { get; set; }

        // Helper properties for the Grid Customization
        [Required]
        [Range(1, 30, ErrorMessage = "Rows must be between 1 and 30")]
        public int Rows { get; set; } = 5;

        [Required]
        [Range(1, 30, ErrorMessage = "Columns must be between 1 and 30")]
        public int Cols { get; set; } = 10;
    }
}