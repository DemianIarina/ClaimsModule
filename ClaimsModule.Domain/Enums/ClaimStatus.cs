using System.Collections.Generic;
using System.Globalization;

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
    /// The claim is currently being processed.
    /// </summary>
    public const string Processing = "Processing";

    /// <summary>
    /// The claim finished processing.
    /// </summary>
    public const string Completed = "Completed";

    /// <summary>
    /// A read-only list of all defined claim statuses.
    /// </summary>
    public static IReadOnlyList<string> All => [Submitted, Processing, Completed];
}
