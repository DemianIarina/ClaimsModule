using Microsoft.AspNetCore.Http;

using System;

/// <summary>
/// Input DTO for submitting a new claim.
/// </summary>
public class CreateClaimRequest
{
    /// <summary>
    /// The ID of the policy on which the claim needs to be made.
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
    /// The type of damage reported in the claim.
    /// </summary>
    public string DamageType { get; set; } = string.Empty;

    /// <summary>
    /// A free-text field where the customer can provide a detailed description of the incident.
    /// </summary>
    public string FullDescription { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether anyone was injured in the incident.
    /// </summary>
    public bool WasAnyoneInjured { get; set; }

    /// <summary>
    /// Indicates whether the vehicle involved had to be towed.
    /// </summary>
    public bool WasTowed { get; set; }

    /// <summary>
    /// Indicates whether alcohol or drugs were involved in the incident.
    /// </summary>
    public bool WasAlcoholOrDrugsInvolved { get; set; }

    /// <summary>
    /// Describes the weather conditions at the time of the incident.
    /// </summary>
    public string WeatherConditions { get; set; } = string.Empty;

    /// <summary>
    /// The name of the person who was driving the vehicle at the time of the incident.
    /// </summary>
    public string WhoWasDriving { get; set; } = string.Empty;

    /// <summary>
    /// Describes the intended use of the vehicle at the time of the incident (e.g., personal, commercial).
    /// </summary>
    public string UseOfVehicle { get; set; } = string.Empty;

    /// <summary>
    /// Lists the areas of the vehicle that were damaged in the incident.
    /// </summary>
    public string AreasDamaged { get; set; } = string.Empty;

    /// <summary>
    /// The estimated cost to repair the reported damages.
    /// </summary>
    public int EstimatedCostOfRepair { get; set; }

    /// <summary>
    /// The current mileage of the vehicle at the time of the incident.
    /// </summary>
    public int CurrentMileage { get; set; }

    /// <summary>
    /// Indicates whether the vehicle's MOT (roadworthiness certificate) was valid at the time of the incident.
    /// </summary>
    public bool WasMOTValid { get; set; }

    /// <summary>
    /// Indicates whether the tires were in proper condition at the time of the incident.
    /// </summary>
    public bool WereTiresProper { get; set; }

    /// <summary>
    /// Indicates whether the brakes were in proper condition at the time of the incident.
    /// </summary>
    public bool WereBreaksProper { get; set; }

    /// <summary>
    /// Indicates whether the lights were in proper working condition at the time of the incident.
    /// </summary>
    public bool WereLightsProper { get; set; }

    /// <summary>
    /// List of image files submitted with the claim.
    /// These photos can help substantiate the reported damages.
    /// </summary>
    public IFormFileCollection? Photos { get; set; }
}
