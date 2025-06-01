using ClaimsModule.Application.Services;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Host.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ClaimsModule.Host.Controllers;

[ApiController]
[Route("/claims")]
public class ClaimController : ControllerBase
{
    private readonly IClaimService _claimService;

    public ClaimController(IClaimService claimService)
    {
        _claimService = claimService;
    }

    /// <summary>
    /// Creates a new claim with structured incident information and uploaded photos.
    /// </summary>
    /// <param name="request">The <see cref="CreateClaimRequest"/> received from the user</param>
    /// <returns>The claim that was created based on the provided information.</returns>
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<CreateClaimResponse>> CreateClaim([FromForm] CreateClaimRequest request)
    {
        // Retrieve customer id
        request.CustomerId = "SubjectInfo";

        // Build semantic description text
        string damageDescription = BuildNarrativeText(request);

        // Create claim entity
        Claim claim = new()
        {
            CustomerId = request.CustomerId!,
            Description = damageDescription,
        };

        Claim createdClaim = await _claimService.CreateClaimAsync(claim, request.Photos);

        return CreatedAtAction(nameof(GetClaimById), new { id = createdClaim.Id }, new CreateClaimResponse
        {
            ClaimId = createdClaim.Id!.Value,
            Status = createdClaim.Status!
        });
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetClaimById(Guid id)
    {
        // optional implementation
        return Ok();
    }

    private string BuildNarrativeText(CreateClaimRequest req)
    {
        var text = $@"
            Incident occurred on {req.IncidentDateTime:yyyy-MM-dd HH:mm} at {req.IncidentLocation}.
            Towed: {(req.WasTowed ? "Yes" : "No")}, Drivable: {(req.IsDrivable ? "Yes" : "No")}.
            Injuries: {(req.WasAnyoneInjured ? "Yes" : "No")}, Other vehicle involved: {(req.WasAnotherVehicleInvolved ? "Yes" : "No")}.
            Police report: {(req.WasPoliceReportFiled ? "Yes" : "No")}, Alcohol/drugs: {(req.WasAlcoholOrDrugsInvolved ? "Yes" : "No")}.
            Weather and road: {req.WeatherConditions}.
            Before: {req.PreIncidentDescription}
            During: {req.IncidentDescription}
            After: {req.PostIncidentDescription}
            Damaged Parts: {req.DamagedParts}
            ";

        return text.Trim();
    }
}
