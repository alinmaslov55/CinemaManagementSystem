using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using CinemaSystem.Models.Entities;

namespace CinemaSystem.Models.ViewModels
{
    public class MovieVM
    {
        public Movie Movie { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> ActorList { get; set; }

        public List<int> SelectedActorIds { get; set; }
    }
}