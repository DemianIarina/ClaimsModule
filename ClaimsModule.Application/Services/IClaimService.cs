using ClaimsModule.Application.RequestModels;
using ClaimsModule.Domain.Entities;
using System.Collections.Generic;
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
    /// <param name="createClaimRequestModel"></param>
    /// <returns>The newly created <see cref="Claim"/> instance.</returns>
    Task<Claim> CreateClaimAsync(CreateClaimRequestModel createClaimRequestModel);

    /// <summary>
    /// Retrieves a claim by id.
    /// </summary>
    /// <param name="id">The id of the claim that needs to be retrieved.</param>
    /// <returns>A <see cref="Claim"/> instance representing the retrieved resource with the provided id, or mull
    /// if the resource was not found.</returns>
    Task<Claim?> GetClaimByIdAsync(string id);

    /// <summary>
    /// Retrieves a list of claims assigned to the specified employee.
    /// </summary>
    /// <param name="employeeId">The identifier of the employee whose assigned claims are to be retrieved.</param>
    /// <returns>List of <see cref="Claim"/> instances assigned to the employee.</returns>
    Task<List<Claim>> GetClaimsByEmpoyeeAsync(string employeeId);

    /// <summary>
    /// Retrieves a list of claims submitted by a specific customer.
    /// </summary>
    /// <param name="customerId">The identifier of the customer whose claims are to be retrieved.</param>
    /// <param name="policyId">The identifier of the policy which claims are to be retrieved.</param>
    /// <returns>List of <see cref="Claim"/> instances created by the customer.</returns>
    Task<List<Claim>> GetClaimsByCustomerAsync(string customerId, string? policyId);

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
