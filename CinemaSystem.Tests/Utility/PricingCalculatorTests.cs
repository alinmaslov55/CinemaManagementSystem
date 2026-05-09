using Xunit;
using FluentAssertions;
using CinemaSystem.Utility;
using CinemaSystem.Models.Data.Enums;

namespace CinemaSystem.Tests.Utility
{
    public class PricingCalculatorTests
    {
        [Theory]
        [InlineData(10.0, null, SeatType.Standard, 10.0)]
        [InlineData(10.0, null, SeatType.Premium, 15.0)]
        [InlineData(10.0, null, SeatType.VIP, 22.0)]
        [InlineData(10.0, null, SeatType.Handicap, 8.0)]
        public void CalculateSeatPrice_UsesMoviePrice_WhenOverrideIsNull(
            double movieBasePrice,
            double? showtimeOverride,
            SeatType seatType,
            double expectedPrice)
        {
            decimal? overrideDecimal = showtimeOverride.HasValue ? (decimal)showtimeOverride.Value : null;
            var result = PricingCalculator.CalculateSeatPrice((decimal)movieBasePrice, overrideDecimal, seatType);

            result.Should().Be((decimal)expectedPrice);
        }

        [Theory]
        [InlineData(15.0, 8.0, SeatType.Standard, 8.0)]
        [InlineData(15.0, 8.0, SeatType.Premium, 13.0)]
        [InlineData(15.0, 8.0, SeatType.VIP, 20.0)]
        [InlineData(15.0, 8.0, SeatType.Handicap, 6.0)]
        public void CalculateSeatPrice_PrioritizesShowtimeOverride_WhenProvided(
            double movieBasePrice,
            double? showtimeOverride,
            SeatType seatType,
            double expectedPrice)
        {
            decimal? overrideDecimal = showtimeOverride.HasValue ? (decimal)showtimeOverride.Value : null;
            var result = PricingCalculator.CalculateSeatPrice((decimal)movieBasePrice, overrideDecimal, seatType);

            result.Should().Be((decimal)expectedPrice);
        }

        [Fact]
        public void CalculateSeatPrice_DefaultsToBasePrice_ForUnknownSeatType()
        {
            var result = PricingCalculator.CalculateSeatPrice(10.0m, null, (SeatType)999);
            result.Should().Be(10.0m);
        }

        [Fact]
        public void CalculateSeatPrice_ReturnsZero_WhenBaseIsLowerThanDiscount()
        {
            var result = PricingCalculator.CalculateSeatPrice(0.0m, null, SeatType.Handicap);

            result.Should().Be(0.0m);
        }
    }
}