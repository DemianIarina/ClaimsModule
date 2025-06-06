using System.Threading.Tasks;

namespace ClaimsModule.Application.Services;

/// <summary>
/// Interface for sending email notifications.
/// Allows sending basic emails with a subject and body to a given recipient.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email asynchronously to the specified recipient.
    /// </summary>
    /// <param name="toEmail">The email address of the recipient.</param>
    /// <param name="subject">The subject line of the email.</param>
    /// <param name="body">The plain text or HTML content of the email body.</param>
    Task SendEmailAsync(string toEmail, string subject, string body);
}