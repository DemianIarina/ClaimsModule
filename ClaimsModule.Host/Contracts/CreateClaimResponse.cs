using System;

namespace ClaimsModule.Host.Contracts;

/// <summary>
/// Output DTO returned after creating a claim.
/// </summary>
public class CreateClaimResponse
{
    /// <summary>
    /// The newly created claim ID.
    /// </summary>
    public Guid ClaimId { get; set; }

    /// <summary>
    /// Current status of the claim (e.g. Submitted).
    /// </summary>
    public string Status { get; set; } = string.Empty;
}
