using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Net.Mail;
using System.Net.Mime;

namespace CinemaSystem.Utility
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
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
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Cinema System", _config["EmailSettings:SenderEmail"]));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };

            if (attachmentBytes != null && !string.IsNullOrEmpty(attachmentName))
            {
                bodyBuilder.Attachments.Add(attachmentName, attachmentBytes, MimeKit.ContentType.Parse("application/pdf"));
            }

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                await client.ConnectAsync(_config["EmailSettings:SmtpServer"], int.Parse(_config["EmailSettings:Port"]), SecureSocketOptions.StartTls);

                await client.AuthenticateAsync(_config["EmailSettings:Username"], _config["EmailSettings:Password"]);

                await client.SendAsync(message);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}