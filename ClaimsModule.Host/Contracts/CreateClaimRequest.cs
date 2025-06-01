using Microsoft.AspNetCore.Http;
using System;

namespace ClaimsModule.Host.Contracts;

/// <summary>
/// Input DTO for submitting a new claim.
/// </summary>
public class CreateClaimRequest
{
    /// <summary>
    /// The ID of the customer submitting the claim.
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    public DateTime IncidentDateTime { get; set; }
    public string IncidentLocation { get; set; } = string.Empty;

    public bool WasTowed { get; set; }
    public bool IsDrivable { get; set; }
    public bool WasAnyoneInjured { get; set; }
    public bool WasAnotherVehicleInvolved { get; set; }
    public bool WasPoliceReportFiled { get; set; }
    public bool WasAlcoholOrDrugsInvolved { get; set; }

    public string WeatherConditions { get; set; } = string.Empty;

    public string PreIncidentDescription { get; set; } = string.Empty;
    public string IncidentDescription { get; set; } = string.Empty;
    public string PostIncidentDescription { get; set; } = string.Empty;

    public string DamagedParts { get; set; } = string.Empty;

    /// <summary>
    /// List of image files submitted with the claim.
    /// </summary>
    public IFormFileCollection? Photos { get; set; }
}
