using ClaimsModule.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClaimsModule.Domain.Entities;

/// <summary>
/// Represents a customer's claim for damage or loss.
/// </summary>
public class Claim
{
    private string? _status;

    /// <summary>
    /// Unique identifier of the claim.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Timestamp when the incident on the claim happened.
    /// </summary>
    public DateTime? IncidentTimestamp { get; set; }

    /// <summary>
    /// The location of the incident
    /// </summary>
    public string? IncidentLocation { get; set; }

    /// <summary>
    /// The type of the damage
    /// </summary>
    public string? DamageType { get; set; }

    /// <summary>
    /// Indicates whether anyone was injured in the incident.
    /// </summary>
    public bool? WasAnyoneInjured { get; set; }

    /// <summary>
    /// Areas of the vehicle that were damaged
    /// </summary>
    public string? AreasDamaged { get; set; }

    /// <summary>
    /// The policy matched with the claim.
    /// </summary>
    public Policy? Policy { get; set; }

    /// <summary>
    /// Free-text description of the damage or event.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Timestamp when the claim was submitted.
    /// </summary>
    public DateTime? SubmittedAt { get; set; }

    /// <summary>
    /// Current status of the claim.
    /// Must have one of the values of <see cref="ClaimStatus"/>
    /// </summary>
    public string? Status
    {
        get => _status;
        set
        {
            if (!ClaimStatus.All.Contains(value))
                throw new ArgumentException($"Invalid policy match status: {value}");

            _status = value;
        }
    }

    /// <summary>
    /// The result of the match of the claim agains the corresponding policy of the customer.
    /// </summary>
    public PolicyMatchResult? PolicyMatchResult { get; set; }

    /// <summary>
    /// The Document generated for a claim.
    /// </summary>
    public PersistedDocument? GeneratedDocument { get; set; }

    /// <summary>
    /// The uploaded photos provided by the customer.
    /// </summary>
    public List<PersistedDocument>? UploadedPhotos { get; set; } = new();

    /// <summary>
    /// The <see cref="Decision"/> made for the claim after automatic review.
    /// </summary>
    public Decision? Decision { get; set; }

    /// <summary>
    ///  The employee assigned to handle this claim.
    /// </summary>
    public Employee? AssignedEmployee { get; set; }
}
