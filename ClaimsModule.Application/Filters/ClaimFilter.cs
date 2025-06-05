namespace ClaimsModule.Application.Filters;

/// <summary>
/// Represents filtering criteria for retrieving claims from the repository.
/// </summary>
public class ClaimFilter
{
    /// <summary>
    /// Optional filter to retrieve only claims associated with a specific customer.
    /// </summary>
    public string? CustomerId { get; set; }

    /// <summary>
    /// Optional filter to retrieve only claims assigned to a specific employee.
    /// </summary>
    public string? EmployeeId { get; set; }
}
