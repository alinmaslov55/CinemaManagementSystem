using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace CinemaSystem.Models.ViewModels
{
    public class CinemaVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Cinema Name is required")]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }
        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [ValidateNever]
        public string? Logo { get; set; }
    }
}
