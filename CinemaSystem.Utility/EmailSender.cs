using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaSystem.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // In the future, you will put your SendGrid or SMTP logic here.
            // For now, we just return a completed task to trick the Register page.
            return Task.CompletedTask;
        }
    }
}
