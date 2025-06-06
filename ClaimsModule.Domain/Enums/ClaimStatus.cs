using System.Collections.Generic;

namespace ClaimsModule.Domain.Enums;

/// <summary>
/// Defines all valid statuses for a claim during its lifecycle.
/// </summary>
public static class ClaimStatus
{
    /// <summary>
    /// The claim was just submitted and stored.
    /// </summary>
    public const string Submitted = "Submitted";

    /// <summary>
    /// The claim has been approved.
    /// </summary>
    public const string Approved = "Approved";

    /// <summary>
    /// The claim has been rejected.
    /// </summary>
    public const string Rejected = "Rejected";

    /// <summary>
    /// The claim has been escalated to human review.
    /// </summary>
    public const string Escalated = "Escalated";

    /// <summary>
    /// A read-only list of all defined claim statuses.
    /// </summary>
    public static IReadOnlyList<string> All => [Submitted, Approved, Rejected, Escalated];
}
