using Microsoft.AspNetCore.Http;
using System;

namespace ClaimsModule.Application.RequestModels;

/// <summary>
/// Request model used to submit a new insurance claim.
/// </summary>
public class CreateClaimRequestModel
{
    /// <summary>
    /// The ID of the customer submitting the claim.
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the policy against which the claim is being submitted.
    /// </summary>
    public string PolicyId { get; set; } = string.Empty;

    /// <summary>
    /// The date and time when the incident occurred.
    /// </summary>
    public DateTime IncidentDateTime { get; set; }

    /// <summary>
    /// The location where the incident occurred.
    /// </summary>
    public string IncidentLocation { get; set; } = string.Empty;

    /// <summary>
    /// A short description of the type of damage reported.
    /// </summary>
    public string DamageType { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether anyone was injured in the incident.
    /// </summary>
    public bool WasAnyoneInjured { get; set; }

    /// <summary>
    /// Description of which areas of the vehicle or property were damaged.
    /// </summary>
    public string AreasDamaged { get; set; } = string.Empty;

    /// <summary>
    /// Free-text narrative describing the full circumstances of the incident.
    /// This combines detailed information about the event, vehicle condition, and any contributing factors.
    /// </summary>
    public string NarrativeText { get; set; } = string.Empty;

    /// <summary>
    /// Optional collection of photos submitted by the customer to support the claim.
    /// </summary>
    public IFormFileCollection? Photos { get; set; }
}
