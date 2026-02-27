using CinemaSystem.Models.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CinemaSystem.Models.ViewModels
{
    public class ShowtimeVM
    {
        public Showtime Showtime { get; set; }

        [Required]
        public DateTime RepeatUntil { get; set; }
        public bool RepeatDaily { get; set; }
        public bool RepeatWeekly { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> MovieList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> CinemaList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> CinemaHallList { get; set; }
    }
}
