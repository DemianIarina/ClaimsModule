using ClaimsModule.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace ClaimsModule.Application.Repositories;

/// <summary>
/// Provides data access operations for insurance policies.
/// </summary>
public interface IPolicyRepository
{
    /// <summary>
    /// Retrieves a policy by its unique identifier.
    /// </summary>
    /// <param name="id">The ID of the policy to retrieve.</param>
    /// <returns>The matching policy, or null if not found.</returns>
    Task<Policy?> GetByIdAsync(string id);
}
