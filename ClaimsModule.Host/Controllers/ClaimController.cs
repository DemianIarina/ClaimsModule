using ClaimsModule.Application.RequestModels;
using ClaimsModule.Application.Services;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Host.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClaimsModule.Host.Controllers;

[ApiController]
[Route("claims")]
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
    [ProducesResponseType(typeof(ClaimResponse), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = "CustomerOnly")]
    public async Task<IActionResult> CreateClaim([FromForm] CreateClaimRequest request)
    {
        string customerId = User.Claims.FirstOrDefault(c => c.Type == "CustomerId")?.Value!;

        // Build semantic description text
        string narrativeText = BuildNarrativeText(request);

        CreateClaimRequestModel createClaimRequestModel = new()
        {
            CustomerId = customerId,
            PolicyId = request.PolicyId,
            IncidentDateTime = request.IncidentDateTime,
            IncidentLocation = request.IncidentLocation,
            DamageType = request.DamageType,
            WasAnyoneInjured = request.WasAnyoneInjured,
            AreasDamaged = request.AreasDamaged,
            NarrativeText = narrativeText,
            Photos = request.Photos
        };

        Claim createdClaim;
        try
        {
            createdClaim = await _claimService.CreateClaimAsync(createClaimRequestModel);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        string? claimUri = Url.Action(nameof(GetClaimById), new { id = createdClaim.Id });

        return Accepted(claimUri, new ClaimResponse
        {
            ClaimId = createdClaim.Id!,
            Status = createdClaim.Status!
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetClaimById(string id)
    {
        var claim = await _claimService.GetClaimByIdAsync(id);

        if (claim == null)
        {
            return NotFound();
        }

        return Ok(new ClaimResponse
        {
            ClaimId = claim.Id!,
            Status = claim.Status!
        });
    }

    /// <summary>
    /// Retrieves all claims assigned to the currently authenticated employee.
    /// </summary>
    /// <returns>A list of claims assigned to the employee.</returns>
    [HttpGet("assigned")]
    [Authorize(Policy = "EmployeeOnly")]
    [ProducesResponseType(typeof(List<ClaimResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAssignedClaims()
    {
        string? employeeId = User.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;

        List<Claim> claims = await _claimService.GetClaimsByEmpoyeeAsync(employeeId!);

        var result = claims.Select(c => new ClaimResponse
        {
            ClaimId = c.Id!,
            Status = c.Status!
        }).ToList();

        return Ok(result);
    }

    /// <summary>
    /// Retrieves all claims submitted by the currently authenticated customer.
    /// </summary>
    /// <returns>A list of claims submitted by the customer.</returns>
    [HttpGet("my")]
    [Authorize(Policy = "CustomerOnly")]
    [ProducesResponseType(typeof(List<ClaimResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyClaims([FromQuery] string? policyId = null)
    {
        string? customerId = User.Claims.FirstOrDefault(c => c.Type == "CustomerId")?.Value;

        List<Claim> claims = await _claimService.GetClaimsByCustomerAsync(customerId!, policyId);

        var result = claims.Select(c => new ClaimResponse
        {
            ClaimId = c.Id!,
            Status = c.Status!
        }).ToList();

        return Ok(result);
    }

    /// <summary>
    /// Manually approves or rejects a claim after manual review.
    /// </summary>
    /// <param name="id">The ID of the claim evaluated.</param>
    /// <param name="request">The manual decision input.</param>
    /// <returns>HTTP 204 No Content on success or 404 if not found.</returns>
    [HttpPost("evaluate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Policy = "EmployeeOnly")]
    public async Task<IActionResult> ManuallyEvaluateClaim([FromBody] ManualEvaluationRequest request)
    {
        string? employeeId = User.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;

        try
        {
            await _claimService.ProcessClaimManuallyAsync(request.ClaimId, request.Approved, employeeId!);
            return Ok();
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"No claim found with ID {request.ClaimId}");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    private string BuildNarrativeText(CreateClaimRequest request)
    {
        return $@"
            Incident Details:
            - Date & Time: {request.IncidentDateTime}
            - Location: {request.IncidentLocation}
            - Damage Type: {request.DamageType}
            - Areas Damaged: {request.AreasDamaged}
            - Was Anyone Injured: {(request.WasAnyoneInjured ? "Yes" : "No")}

            Additional Information:
            - Was Towed: {(request.WasTowed ? "Yes" : "No")}
            - Alcohol/Drugs Involved: {(request.WasAlcoholOrDrugsInvolved ? "Yes" : "No")}
            - Weather Conditions: {request.WeatherConditions}
            - Who Was Driving: {request.WhoWasDriving}
            - Use of Vehicle: {request.UseOfVehicle}
            - Estimated Cost of Repair: {request.EstimatedCostOfRepair} EUR
            - Current Mileage: {request.CurrentMileage} km
            - MOT Valid: {(request.WasMOTValid ? "Yes" : "No")}
            - Tires Proper: {(request.WereTiresProper ? "Yes" : "No")}
            - Brakes Proper: {(request.WereBreaksProper ? "Yes" : "No")}
            - Lights Proper: {(request.WereLightsProper ? "Yes" : "No")}

            Full Description Provided by Customer:
            {request.FullDescription}
            ";
    }
}
