using ClaimsModule.Domain.Entities;
using System.Threading.Tasks;

namespace ClaimsModule.Application.Repositories;

/// <summary>
/// Provides data access operations for  claims entities.
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
    Task<Claim?> GetByIdAsync(string id);

    /// <summary>
    /// Updates an existing claim and any of its related entities (such as decisions or match results).
    /// </summary>
    /// <param name="claim">The <see cref="Claim"/> entity to be updated.</param>
    Task UpdateAsync(Claim claim);
}
