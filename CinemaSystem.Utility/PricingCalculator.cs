using CinemaSystem.Models.Data.Enums;
using System;

namespace CinemaSystem.Utility
{
    public static class PricingCalculator
    {
        public static decimal CalculateSeatPrice(decimal movieBasePrice, decimal? showtimePriceOverride, SeatType seatType)
        {
            decimal basePrice = showtimePriceOverride ?? movieBasePrice;

            decimal calculatedPrice = seatType switch
            {
                SeatType.Standard => basePrice,
                SeatType.Premium => basePrice + 5.00m,
                SeatType.VIP => basePrice + 12.00m,
                SeatType.Handicap => basePrice - 2.00m,
                _ => basePrice
            };

            return Math.Max(0m, calculatedPrice);
        }
    }
}