using CinemaSystem.Models.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CinemaSystem.Models.ViewModels
{
    public class EquipmentVM
    {
        public Equipment Equipment { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> HallList { get; set; }
    }
}