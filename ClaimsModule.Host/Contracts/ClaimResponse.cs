using System;

namespace ClaimsModule.Host.Contracts;

/// <summary>
/// Output DTO returned when retrieving a claim or after creating a claim.
/// </summary>
public class ClaimResponse
{
    /// <summary>
    /// The unique identifier of the claim.
    /// </summary>
    public string ClaimId { get; set; } = string.Empty;

    /// <summary>
    /// The date and time when the incident occurred.
    /// </summary>
    public DateTime IncidentDateTime { get; set; }

    /// <summary>
    /// The location where the incident occurred.
    /// </summary>
    public string IncidentLocation { get; set; } = string.Empty;

    /// <summary>
    /// The type of damage reported.
    /// </summary>
    public string DamageType { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether anyone was injured in the incident.
    /// </summary>
    public bool WasAnyoneInjured { get; set; }

    /// <summary>
    /// Areas of the vehicle or property that were damaged.
    /// </summary>
    public string AreasDamaged { get; set; } = string.Empty;

    /// <summary>
    /// Current status of the claim (e.g. Submitted, Approved, Escalated, Rejected).
    /// </summary>
    public string Status { get; set; } = string.Empty;
}
