using CinemaSystem.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaSystem.Models.ViewModels
{
    public class CheckoutVM
    {
        public Showtime Showtime { get; set; }
        public List<TicketItemDto> TicketItems { get; set; } = new List<TicketItemDto>();
        public decimal TotalAmount { get; set; }
    }

    public class TicketItemDto
    {
        public int SeatId { get; set; }
        public string SeatName { get; set; } // "A1"
        public decimal Price { get; set; }
    }
}
