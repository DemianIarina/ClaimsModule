using ClaimsModule.Domain.Entities;
using System.Threading.Tasks;

namespace ClaimsModule.Application.Repositories;

/// <summary>
/// Provides access to application user persistence.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user entity by its unique identifier.
    /// </summary>
    /// <param name="customerId">The unique identifier of the user.</param>
    /// <returns>The <see cref="AppUser"/> if found; otherwise, null.</returns>
    Task<AppUser?> GetByUsernameAsync(string username);
}
