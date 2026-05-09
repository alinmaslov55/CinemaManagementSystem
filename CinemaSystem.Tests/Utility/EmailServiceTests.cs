using CinemaSystem.Utility;
using FluentAssertions;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CinemaSystem.Tests.Utility
{
    public class EmailServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<ILogger<EmailService>> _mockLogger;
        private readonly Mock<ISmtpClient> _mockSmtpClient;
        private readonly EmailService _emailService;

        public EmailServiceTests()
        {
            _mockConfig = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<EmailService>>();
            _mockSmtpClient = new Mock<ISmtpClient>();

            _mockConfig.Setup(c => c["EmailSettings:SenderEmail"]).Returns("admin@cinema.com");
            _mockConfig.Setup(c => c["EmailSettings:SmtpServer"]).Returns("smtp.test.com");
            _mockConfig.Setup(c => c["EmailSettings:Port"]).Returns("587");
            _mockConfig.Setup(c => c["EmailSettings:Username"]).Returns("testuser");
            _mockConfig.Setup(c => c["EmailSettings:Password"]).Returns("testpass");

            _mockSmtpClient.Setup(c => c.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<SecureSocketOptions>(), It.IsAny<CancellationToken>()))
                           .Returns(Task.CompletedTask);
            _mockSmtpClient.Setup(c => c.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                           .Returns(Task.CompletedTask);
            _mockSmtpClient.Setup(c => c.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), It.IsAny<ITransferProgress>()))
                           .ReturnsAsync("MockResponse");

            _mockSmtpClient.SetupGet(c => c.IsConnected).Returns(true);

            _emailService = new EmailService(_mockConfig.Object, _mockLogger.Object, _mockSmtpClient.Object);
        }

        [Fact]
        public async Task SendEmailAsync_OrchestratesSmtpClientCorrectly_WhenSuccessful()
        {
            await _emailService.SendEmailAsync("client@test.com", "Test Subject", "<h1>Hello</h1>");

            _mockSmtpClient.Verify(c => c.ConnectAsync("smtp.test.com", 587, SecureSocketOptions.StartTls, It.IsAny<CancellationToken>()), Times.Once);
            _mockSmtpClient.Verify(c => c.AuthenticateAsync("testuser", "testpass", It.IsAny<CancellationToken>()), Times.Once);

            _mockSmtpClient.Verify(c => c.SendAsync(It.Is<MimeMessage>(m =>
                m.To.Mailboxes.First().Address == "client@test.com" &&
                m.Subject == "Test Subject" &&
                m.HtmlBody == "<h1>Hello</h1>"
            ), It.IsAny<CancellationToken>(), It.IsAny<ITransferProgress>()), Times.Once);

            _mockSmtpClient.Verify(c => c.DisconnectAsync(true, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SendEmailWithAttachmentAsync_AddsPdfToMimeMessage()
        {
            byte[] fakePdfBytes = new byte[] { 0x25, 0x50, 0x44, 0x46 };
            string fileName = "tickets.pdf";

            await _emailService.SendEmailWithAttachmentAsync("client@test.com", "Your Tickets", "<p>Enjoy</p>", fakePdfBytes, fileName);

            _mockSmtpClient.Verify(c => c.SendAsync(It.Is<MimeMessage>(m =>
                m.Attachments.Count() == 1 &&
                m.Attachments.First().ContentDisposition.FileName == fileName
            ), It.IsAny<CancellationToken>(), It.IsAny<ITransferProgress>()), Times.Once);
        }

        [Fact]
        public async Task SendEmailAsync_LogsErrorAndRethrows_WhenNetworkFails()
        {
            _mockSmtpClient.Setup(c => c.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<SecureSocketOptions>(), It.IsAny<CancellationToken>()))
                           .ThrowsAsync(new System.Net.Sockets.SocketException());

            Func<Task> act = async () => await _emailService.SendEmailAsync("client@test.com", "Fail Subject", "Body");

            await act.Should().ThrowAsync<System.Net.Sockets.SocketException>();

            _mockSmtpClient.Verify(c => c.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockSmtpClient.Verify(c => c.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), It.IsAny<ITransferProgress>()), Times.Never);
        }
    }
}