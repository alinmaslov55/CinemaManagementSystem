using CinemaSystem.Models.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaSystem.Utility
{
    public static class PricingCalculator
    {
        /// <summary>
        /// Calculates the final price of a single seat for a specific showtime.
        /// </summary>
        /// <param name="movieBasePrice">The default price of the movie.</param>
        /// <param name="showtimePriceOverride">The optional overridden price for the specific showtime.</param>
        /// <param name="seatType">The physical type of the seat being purchased.</param>
        /// <returns>The final calculated price.</returns>
        public static decimal CalculateSeatPrice(decimal movieBasePrice, decimal? showtimePriceOverride, SeatType seatType)
        {
            decimal basePrice = showtimePriceOverride ?? movieBasePrice;

            return seatType switch
            {
                SeatType.Standard => basePrice,
                SeatType.Premium => basePrice + 5.00m,
                SeatType.VIP => basePrice + 12.00m,
                SeatType.Handicap => basePrice - 2.00m,
                _ => basePrice
            };
        }
    }
}
