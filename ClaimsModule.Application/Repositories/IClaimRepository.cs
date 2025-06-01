using ClaimsModule.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace ClaimsModule.Application.Repositories;

/// <summary>
/// Defines data access operations for claims entities.
/// </summary>
public interface IClaimRepository
{
    /// <summary>
    /// Adds a new claim to the data store.
    /// </summary>
    /// <param name="claim">The <see cref="Claim"/> entity to be persisted.</param>
    Task AddAsync(Claim claim);

    /// <summary>
    /// Retrieves a claims based on its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the claim.</param>
    /// <returns>The <see cref="Claim"/>, or null if no claim was found for the given ID.</returns>
    Task<Claim?> GetByIdAsync(Guid id);
}
