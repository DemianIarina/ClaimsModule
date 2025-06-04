using ClaimsModule.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace ClaimsModule.Application.Services;

/// <summary>
/// Defines operations for handling the lifecycle of insurance claims.
/// </summary>
public interface IClaimService
{
    /// <summary>
    /// Creates a new claim from structured input, including file attachments and metadata.
    /// </summary>
    /// <param name="policyId">Id of the <see cref="Policy"/> on which the claim is made.</param>
    /// <param name="customerId">Id of the <see cref="Customer"/> who makes the claim.</param>
    /// <param name="incidentTimestamp">Timestamp when the incident on the claim happened.</param>
    /// <param name="description">Narrative description of what happend in the incident.</param>
    /// <param name="photos">Optional photos provided of the damage.</param>
    /// <returns></returns>
    Task<Claim> CreateClaimAsync(string policyId, string customerId, DateTime incidentTimestamp, string description, IFormFileCollection? photos);

    /// <summary>
    /// Retrieves a claim by id.
    /// </summary>
    /// <param name="id">The id of the claim that needs to be retrieved.</param>
    /// <returns>A <see cref="Claim"/> instance representing the retrieved resource with the provided id, or mull
    /// if the resource was not found.</returns>
    Task<Claim?> GetClaimByIdAsync(string id);

    /// <summary>
    /// Starts processing logic for an insurance claim asynchronously.
    /// </summary>
    /// <param name="id">Id of the claim that needs to be processed</param>
    Task ProcessClaimAutomaticallyAsync(string id);

    /// <summary>
    /// Processes a claim based on manual evaluation by an employee.
    /// </summary>
    /// <param name="claimId">The identifier of the claim to process.</param>
    /// <param name="approved">Indicates whether the claim is approved or rejected.</param>
    /// <param name="employeeId">The identifier of the employee performing the evaluation.</param>
    Task ProcessClaimManuallyAsync(string claimId, bool approved, string employeeId);
}
