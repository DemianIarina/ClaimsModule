using ClaimsModule.Domain.Entities;
using Microsoft.AspNetCore.Http;
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
}
