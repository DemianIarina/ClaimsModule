using ClaimsModule.Application.Repositories;
using ClaimsModule.Application.Services;
using ClaimsModule.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Claim = System.Security.Claims.Claim;

namespace ClaimsModule.Infrastructure.Services;

/// <summary>
/// Service responsible for handling user authentication and JWT token generation.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _config;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthService"/> class.
    /// </summary>
    /// <param name="userRepository">The repository used to fetch user credentials.</param>
    /// <param name="config">Application configuration used for retrieving JWT settings.</param>
    public AuthService(IUserRepository userRepository, IConfiguration config)
    {
        _userRepository = userRepository;
        _config = config;
    }

    /// <inheritdoc/>
    public async Task<string> AuthenticateAsync(string username, string password)
    {
        var emailValidator = new EmailAddressAttribute();
        if (!emailValidator.IsValid(username))
        {
            throw new ArgumentException("Username must be a valid email address.");
        }

        AppUser? user = await _userRepository.GetByUsernameAsync(username);
        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        return GenerateJwtToken(user);
    }

    /// <summary>
    /// Generates a JWT token containing user claims and authorization info.
    /// </summary>
    /// <param name="user">The authenticated user for whom to generate the token.</param>
    /// <returns>A JWT token string.</returns>
    private string GenerateJwtToken(AppUser user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("CustomerId", user.LinkedCustomerId ?? ""),
            new Claim("EmployeeId", user.LinkedEmployeeId ?? "")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
