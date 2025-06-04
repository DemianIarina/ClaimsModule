using ClaimsModule.Application.Repositories;
using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ClaimsModule.Infrastructure.Repositories;

/// <summary>
/// Provides access to user data for authentication and authorization.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly ClaimsDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRepository"/> class.
    /// </summary>
    /// <param name="context">The database context used to access user data.</param>
    public UserRepository(ClaimsDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a user by their username.
    /// </summary>
    /// <param name="username">The username of the user to retrieve.</param>
    /// <returns>The matching <see cref="AppUser"/> if found; otherwise, null.</returns>
    public async Task<AppUser?> GetByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }
}
