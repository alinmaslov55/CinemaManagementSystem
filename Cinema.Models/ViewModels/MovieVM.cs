using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Cinema.Models.Data.Enums;

namespace Cinema.Models.ViewModels
{
    public class MovieVM
    {
        public int Id { get; set; } // Needed for Edit operations

        [Required(ErrorMessage = "Title is required")]
        [Display(Name = "Movie Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(1, 500)]
        public double Price { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public MovieCategory Category { get; set; }

        [Required(ErrorMessage = "Please select at least one actor")]
        [Display(Name = "Select Actor(s)")]
        public List<int> SelectedActorIds { get; set; }

        [Required(ErrorMessage = "Please select a cinema")]
        [Display(Name = "Cinema Location")]
        public int SelectedCinemaId { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [Display(Name = "Duration (Minutes)")]
        public int Duration { get; set; }

        [Display(Name = "Trailer URL")]
        public string? TrailerUrl { get; set; }

        public string? Description { get; set; }

        // Dropdown data (Not mapped to DB)
        public IEnumerable<SelectListItem>? ActorList { get; set; }
        public IEnumerable<SelectListItem>? CinemaList { get; set; }
    }
}