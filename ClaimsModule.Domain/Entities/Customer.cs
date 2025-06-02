using System.Collections.Generic;

namespace ClaimsModule.Domain.Entities;

/// <summary>
/// Represents a customer.
/// </summary>
public class Customer
{
    /// <summary>
    /// Unique identifier of the customer.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Full name of the customer.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Email address of the customer.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// List of insurance policies held by the customer.
    /// </summary>
    public IList<Policy> Policies { get; set; } = new List<Policy>();
}
