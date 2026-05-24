using CinemaSystem.Utility;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace CinemaSystem.Utility
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await ExecuteSendAsync(email, subject, htmlMessage, null, null);
        }

        public async Task SendEmailWithAttachmentAsync(string email, string subject, string htmlMessage, byte[] attachmentBytes, string attachmentName)
        {
            await ExecuteSendAsync(email, subject, htmlMessage, attachmentBytes, attachmentName);
        }

        private async Task ExecuteSendAsync(string email, string subject, string htmlMessage, byte[]? attachmentBytes, string? attachmentName)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Ethereal Cinema", _config["EmailSettings:SenderEmail"]));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };

                if (attachmentBytes != null && !string.IsNullOrWhiteSpace(attachmentName))
                {
                    bodyBuilder.Attachments.Add(attachmentName, attachmentBytes, ContentType.Parse("application/pdf"));
                }

                message.Body = bodyBuilder.ToMessageBody();

                using var smtpClient = new SmtpClient();

                await smtpClient.ConnectAsync(
                    _config["EmailSettings:SmtpServer"],
                    int.Parse(_config["EmailSettings:Port"]),
                    SecureSocketOptions.StartTls);

                await smtpClient.AuthenticateAsync(
                    _config["EmailSettings:Username"],
                    _config["EmailSettings:Password"]);

                await smtpClient.SendAsync(message);

                await smtpClient.DisconnectAsync(true);

                _logger.LogInformation("Successfully sent email to {Email} with subject {Subject}", email, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}. Subject: {Subject}", email, subject);
                throw;
            }
        }
    }
}