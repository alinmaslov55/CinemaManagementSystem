using CinemaSystem.Models.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaSystem.Utility
{
    public static class PricingCalculator
    {
        public static decimal GetSeatPrice(decimal basePrice, SeatType type)
        {
            return type switch
            {
                SeatType.Premium => basePrice + 5.00m,
                SeatType.VIP => basePrice + 12.00m,
                SeatType.Handicap => basePrice - 2.00m,
                _ => basePrice // Standard
            };
        }
    }
}
