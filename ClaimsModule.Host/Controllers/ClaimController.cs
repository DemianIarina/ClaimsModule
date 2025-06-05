using ClaimsModule.Application.Services;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Host.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

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
        string? customerId = User.Claims.FirstOrDefault(c => c.Type == "CustomerId")?.Value;

        // Build semantic description text
        string damageDescription = BuildNarrativeText(request);

        Claim createdClaim;
        try
        {
            createdClaim = await _claimService.CreateClaimAsync(request.PolicyId, customerId!, request.IncidentDateTime,
                damageDescription, request.Photos);
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
    public async Task<IActionResult> GetMyClaims()
    {
        string? customerId = User.Claims.FirstOrDefault(c => c.Type == "CustomerId")?.Value;

        List<Claim> claims = await _claimService.GetClaimsByCustomerAsync(customerId!);

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
