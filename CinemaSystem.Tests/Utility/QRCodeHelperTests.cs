using Xunit;
using FluentAssertions;
using System;
using CinemaSystem.Utility;

namespace CinemaSystem.Tests.Utility
{
    public class QRCodeHelperTests
    {
        [Fact]
        public void GenerateQRCodeBytes_ReturnsValidPngByteArray_WhenPayloadIsValid()
        {
            string validPayload = "TICKET-12345-ABC";

            var result = QRCodeHelper.GenerateQRCodeBytes(validPayload);

            result.Should().NotBeNull();
            result.Should().NotBeEmpty();

            result[0].Should().Be(137);
            result[1].Should().Be(80); // 'P'
            result[2].Should().Be(78); // 'N'
            result[3].Should().Be(71); // 'G'

            result.Length.Should().BeGreaterThan(100);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GenerateQRCodeBytes_ThrowsArgumentException_WhenPayloadIsInvalid(string invalidPayload)
        {
            Action act = () => QRCodeHelper.GenerateQRCodeBytes(invalidPayload);

            act.Should().Throw<ArgumentException>()
               .WithMessage("*Payload cannot be null, empty, or whitespace.*")
               .WithParameterName("payload");
        }
    }
}