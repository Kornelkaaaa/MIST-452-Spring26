using Microsoft.AspNetCore.Identity.UI.Services;

namespace BooksSpring26
{
    public class EmailSender : IEmailSender
    {
        Task IEmailSender.SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Implement your email sending logic here
            // For example, you can use an SMTP client or a third-party email service API
            // This is a placeholder implementation that simply returns a completed task
            return Task.CompletedTask;
        }
    }
}
