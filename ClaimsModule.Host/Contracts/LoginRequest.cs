namespace ClaimsModule.Host.Contracts;

/// <summary>
/// Represents the payload for authenticating a user.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// The username of the user trying to authenticate.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The password of the user trying to authenticate.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
