using System.Collections.Generic;

namespace ClaimsModule.Host.Contracts;

/// <summary>
/// Represents a data transfer object that encapsulates a customer and their associated insurance policies.
/// Used for API responses that return both customer details and related policy data.
/// </summary>
public class CustomerWithPoliciesDto
{
    /// <summary>
    /// Customer information
    /// </summary>
    public CustomerDto Customer { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of insurance policies associated with the customer.
    /// </summary>
    public List<PolicyDto> Policies { get; set; } = new();
}