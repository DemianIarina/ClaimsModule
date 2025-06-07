using System;
using System.Collections.Generic;

namespace ClaimsModule.Host.Contracts;

/// <summary>
/// Output DTO returned when retrieving a claim or after creating a claim.
/// </summary>
public class ClaimResponse
{
    /// <summary>
    /// The unique identifier of the claim.
    /// </summary>
    public string? ClaimId { get; set; }

    /// <summary>
    /// The date and time when the incident occurred.
    /// </summary>
    public DateTime? IncidentDateTime { get; set; }
     
    /// <summary>
    /// The location where the incident occurred.
    /// </summary>
    public string? IncidentLocation { get; set; }

    /// <summary>
    /// The type of damage reported.
    /// </summary>
    public string? DamageType { get; set; }

    /// <summary>
    /// Indicates whether anyone was injured in the incident.
    /// </summary>
    public bool? WasAnyoneInjured { get; set; }

    /// <summary>
    /// Areas of the vehicle or property that were damaged.
    /// </summary>
    public string? AreasDamaged { get; set; }

    /// <summary>
    /// Current status of the claim (e.g. Submitted, Approved, Escalated, Rejected).
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Name of the customer who submitted the claim.
    /// </summary>
    public string? CustomerName {  get; set; }

    /// <summary>
    /// List of urls of the attached photos
    /// </summary>
    public List<string>? AttachedPhotosUrls { get; set; }

    /// <summary>
    /// Url of the generated document, in case the claim was accepted
    /// </summary>
    public string? GeneratedDocumentUrl {  get; set; }

    /// <summary>
    /// The full description of the claim, as a narrative text
    /// </summary>
    public string? FullDescription { get; set; }
}
