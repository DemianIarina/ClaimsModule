namespace ClaimsModule.Infrastructure.Config;

/// <summary>
/// Represents configuration settings for the SMTP server used to send emails.
/// These settings can be configured in appsettings.json or an external configuration source.
/// </summary>
public class SmtpSettings
{
    /// <summary>
    /// The hostname or IP address of the SMTP server (e.g. smtp.gmail.com).
    /// </summary>
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// The port number used to connect to the SMTP server (default is 587 for STARTTLS).
    /// </summary>
    public int Port { get; set; } = 587;

    /// <summary>
    /// The username used to authenticate with the SMTP server.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The password (or app password) used to authenticate with the SMTP server.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether to enable SSL/TLS encryption when sending emails.
    /// </summary>
    public bool EnableSsl { get; set; } = true;
}