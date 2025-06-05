using System.Collections.Generic;

namespace ClaimsModule.Domain.Entities;

/// <summary>
/// Represents an employee who can manually review and evaluate claims.
/// </summary>
public class Employee
{
    /// <summary>
    /// Unique identifier of the employee.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Full name of the employee.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Email address of the employee.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Policies this employee is responsible for.
    /// </summary>
    public IList<Policy> Policies { get; set; } = new List<Policy>();

    /// <summary>
    /// List of claims the employee has manually reviewed or is responsible for.
    /// </summary>
    public IList<Claim> AssignedClaims { get; set; } = new List<Claim>();
}
