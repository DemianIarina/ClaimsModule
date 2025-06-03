namespace ClaimsModule.Host.Contracts;

/// <summary>
/// Represents the request payload for manually evaluating a claim by an employee.
/// </summary>
public class ManualEvaluationRequest
{
    /// <summary>
    /// Indicates whether the claim is approved or rejected.
    /// </summary>
    public bool Approved { get; set; }

    /// <summary>
    /// The unique identifier of the employee performing the manual evaluation.
    /// </summary>
    public string EmployeeId { get; set; } = string.Empty;
}