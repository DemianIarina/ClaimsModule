using System;

namespace ClaimsModule.Host.Contracts;

/// <summary>
/// Represents a data transfer object (DTO) containing basic information about a policy.
/// Used for exposing policy data via API responses.
/// </summary>
public class PolicyDto
{
    /// <summary>
    /// Unique identifier of the policy.
    /// </summary>
    public string? Id { get; set; }
    /// <summary>
    /// Public-facing policy number used for reference.
    /// </summary>
    public string? PolicyNumber { get; set; }

    /// <summary>
    /// License plate number of the insured vehicle.
    /// </summary>
    public string? CarPlateNumber { get; set; }

    /// <summary>
    /// Brand of the insured vehicle.
    /// </summary>
    public string? CarBrand { get; set; }

    /// <summary>
    /// Model of the insured vehicle.
    /// </summary>
    public string? CarModel { get; set; }

    /// <summary>
    /// Start date of the policy’s coverage period.
    /// </summary>
    public DateTime? ValidFrom { get; set; }

    /// <summary>
    /// End date of the policy’s coverage period.
    /// </summary>
    public DateTime? ValidTo { get; set; }

    /// <summary>
    /// URL reference to the official digital policy document.
    /// </summary>
    public string? DocumentReference { get; set; }
}
