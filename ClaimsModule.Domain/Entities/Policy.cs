using System;

namespace ClaimsModule.Domain.Entities;

/// <summary>
/// Represents an insurance policy.
/// </summary>
public class Policy
{
    /// <summary>
    /// Unique identifier of the policy.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Number of the policy, used to externally identify the policy
    /// </summary>
    public string? PolicyNumber { get; set; }

    /// <summary>
    /// The plate number of the cars
    /// </summary>
    public string? CarPlateNumber { get; set; }

    /// <summary>
    /// Brand of the car covered by the policy (e.g., Toyota, Ford).
    /// </summary>
    public string? CarBrand { get; set; }

    /// <summary>
    /// Model of the car covered by the policy (e.g., Corolla, Focus).
    /// </summary>
    public string? CarModel { get; set; }

    /// <summary>
    /// The start date of the policy's validity period.
    /// </summary>
    public DateTime ValidFrom { get; set; }

    /// <summary>
    /// The end date of the policy's validity period.
    /// </summary>
    public DateTime ValidTo { get; set; }

    /// <summary>
    /// The customer to whom the policy is assigned.
    /// </summary>
    public Customer? Customer { get; set; }

    /// <summary>
    /// The employee responsible for managing this policy.
    /// </summary>
    public Employee? ResponsibleEmployee { get; set; }

    /// <summary>
    /// The <see cref="PersistedDocument"/> representing the policy document.
    /// </summary>
    public PersistedDocument? PolicyDocument { get; set; }
}

