using System;

namespace ClaimsModule.Domain.Entities;

/// <summary>
/// Represents a user account in the system used for authentication and authorization.
/// Can be linked to a customer or employee depending on the role.
/// </summary>
public class AppUser
{
    /// <summary>
    /// Unique identifier of the user.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Login username used to authenticate the user.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Hashed password used to validate credentials securely.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Role assigned to the user (e.g., "Customer" or "Employee").
    /// </summary>
    public string Role { get; set; } = "Customer";

    /// <summary>
    /// Optional reference to the customer entity this user represents.
    /// </summary>
    public string? LinkedCustomerId { get; set; }

    /// <summary>
    /// Optional reference to the employee entity this user represents.
    /// </summary>
    public string? LinkedEmployeeId { get; set; }
}