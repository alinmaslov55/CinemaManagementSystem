using CinemaSystem.Utility;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace CinemaSystem.Utility
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        // Injected Logger to track SMTP failures
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

                // Handle optional attachments safely
                if (attachmentBytes != null && !string.IsNullOrWhiteSpace(attachmentName))
                {
                    bodyBuilder.Attachments.Add(attachmentName, attachmentBytes, ContentType.Parse("application/pdf"));
                }

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                // Connect
                await client.ConnectAsync(
                    _config["EmailSettings:SmtpServer"],
                    int.Parse(_config["EmailSettings:Port"]),
                    SecureSocketOptions.StartTls);

                // Authenticate
                await client.AuthenticateAsync(
                    _config["EmailSettings:Username"],
                    _config["EmailSettings:Password"]);

                // Send
                await client.SendAsync(message);

                _logger.LogInformation("Successfully sent email to {Email} with subject {Subject}", email, subject);
            }
            catch (Exception ex)
            {
                // We catch and log the error so it doesn't crash the calling thread (e.g., during user registration)
                _logger.LogError(ex, "Failed to send email to {Email}. Subject: {Subject}", email, subject);
                throw; // Re-throw if you want the calling controller to know it failed, or remove 'throw' to fail silently
            }
            finally
            {
                // Disconnect is only valid if we successfully created the client
                // Ensure the client is properly disconnected
                // Note: The 'using' statement handles disposal, but explicit disconnect is good practice in MailKit
            }
        }
    }
}