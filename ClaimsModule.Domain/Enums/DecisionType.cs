using System.Collections.Generic;

namespace ClaimsModule.Domain.Enums;

/// <summary>
/// Defines types of decisions taken after claim evaluation.
/// </summary>
public static class DecisionType
{
    /// <summary>
    /// The claim has been approved.
    /// </summary>
    public const string Approved = "Approved";

    /// <summary>
    /// The claim has been rejected due to mismatch or policy conflict.
    /// </summary>
    public const string Rejected = "Rejected";

    /// <summary>
    /// The claim has been escalated to human review.
    /// </summary>
    public const string Escalated = "Escalated";

    /// <summary>
    /// Returns all allowed decision types.
    /// </summary>
    public static IReadOnlyList<string> All => [Approved, Rejected, Escalated];
}
