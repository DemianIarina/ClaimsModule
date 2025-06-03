namespace ClaimsModule.Host.Contracts;

/// <summary>
/// Represents a data transfer object (DTO) containing basic information about a customer.
/// Used for exposing customer data via API responses.
/// </summary>
public class CustomerDto
{
    /// <summary>
    /// Unique identifier of the customer
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Full name of the customer
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Email address of the customer
    /// </summary>
    public string Email { get; set; } = string.Empty;
}