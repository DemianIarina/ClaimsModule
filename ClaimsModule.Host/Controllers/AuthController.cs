using ClaimsModule.Application.Services;
using ClaimsModule.Host.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ClaimsModule.Host.Controllers;

/// <summary>
/// Exposes endpoints for authenticating users and generating JWT tokens.
/// </summary>
[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Authenticates a user and issues a JWT token upon successful login.
    /// </summary>
    /// <param name="request">The login request containing the username and password.</param>
    /// <returns>An HTTP 200 response containing a JWT token if authentication succeeds; otherwise, 401 Unauthorized.</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var token = await _authService.AuthenticateAsync(request.Username, request.Password);
            return Ok(new { token });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }
}
