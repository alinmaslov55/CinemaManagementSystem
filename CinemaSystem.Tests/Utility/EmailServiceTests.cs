using CinemaSystem.Utility;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CinemaSystem.Tests.Utility
{
    public class EmailServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<ILogger<EmailService>> _mockLogger;
        private readonly EmailService _emailService;

        public EmailServiceTests()
        {
            _mockConfig = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<EmailService>>();

            _mockConfig.Setup(c => c["EmailSettings:SenderEmail"]).Returns("admin@cinema.com");
            _mockConfig.Setup(c => c["EmailSettings:SmtpServer"]).Returns("invalid.smtp.local");
            _mockConfig.Setup(c => c["EmailSettings:Port"]).Returns("587");
            _mockConfig.Setup(c => c["EmailSettings:Username"]).Returns("testuser");
            _mockConfig.Setup(c => c["EmailSettings:Password"]).Returns("testpass");

            _emailService = new EmailService(_mockConfig.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task SendEmailAsync_ActsAsIntegrationTest_ThrowsExceptionWhenServerIsUnreachable()
        {
            // Act
            Func<Task> act = async () => await _emailService.SendEmailAsync("client@test.com", "Test Subject", "<h1>Hello</h1>");

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task SendEmailWithAttachmentAsync_ThrowsException_WhenAttemptingRealNetworkCall()
        {
            // Arrange
            byte[] fakePdfBytes = new byte[] { 0x25, 0x50, 0x44, 0x46 };

            // Act
            Func<Task> act = async () => await _emailService.SendEmailWithAttachmentAsync(
                "client@test.com", "Tickets", "<p>Enjoy</p>", fakePdfBytes, "tickets.pdf");

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }
    }
}