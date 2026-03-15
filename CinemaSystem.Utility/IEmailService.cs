using Microsoft.AspNetCore.Identity.UI.Services;

namespace CinemaSystem.Utility
{
    public interface IEmailService : IEmailSender
    {
        Task SendEmailWithAttachmentAsync(string email, string subject, string htmlMessage, byte[] attachmentBytes, string attachmentName);
    }
}