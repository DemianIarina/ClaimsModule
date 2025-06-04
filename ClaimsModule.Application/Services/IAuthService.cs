using System.Threading.Tasks;

namespace ClaimsModule.Application.Services;

/// <summary>
/// Provides user authentication operations for login and token generation.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Validates the user's credentials and returns a JWT if valid.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="password">The plain text password.</param>
    /// <returns>JWT token string if authentication is successful.</returns>
    Task<string> AuthenticateAsync(string username, string password);
}