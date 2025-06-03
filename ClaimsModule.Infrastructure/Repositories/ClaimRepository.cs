using ClaimsModule.Application.Repositories;
using System.Threading.Tasks;
using System;
using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Infrastructure.Repositories;

/// <summary>
/// Entity Framework implementation of <see cref="IClaimRepository"/> for accessing claim data.
/// </summary>
public class ClaimRepository : IClaimRepository
{
    private readonly ClaimsDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClaimRepository"/> class.
    /// </summary>
    /// <param name="context">The database context for claim storage.</param>
    public ClaimRepository(ClaimsDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task AddAsync(Claim claim)
    {
        _context.Claims.Add(claim);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<Claim?> GetByIdAsync(string id)
    {
        return await _context.Claims
            .Include(c => c.Policy)
                .ThenInclude(p => p!.Customer)
            .Include(c => c.Decision)
            .Include(c => c.GeneratedDocument)
            .Include(c => c.PolicyMatchResult)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task UpdateAsync(Claim claim)
    {
        _context.Claims.Update(claim); 
        await _context.SaveChangesAsync();
    }
}
