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
    /// <param name="claim"> An object containing the User Id and detailed incident information
    /// (e.g. time, location, vehicle condition) as a narrative descriptions
    /// </param>
    /// <param name="photos">Optional photos provided of the damage.</param>
    /// <returns>
    /// A <see cref="Claim"/> instance representing the newly created claim, including its assigned ID and initial status.
    /// </returns>
    Task<Claim> CreateClaimAsync(Claim claim, IFormFileCollection? photos);

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
    Task ProcessClaimAsync(string id);
}
