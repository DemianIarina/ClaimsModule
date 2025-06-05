using ClaimsModule.Application.Filters;
using ClaimsModule.Application.Repositories;
using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                .Include(c => c.AssignedEmployee)
            .Include(c => c.Decision)
            .Include(c => c.GeneratedDocument)
            .Include(c => c.PolicyMatchResult)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Claim>> GetListAsync(ClaimFilter filter)
    {
        IQueryable<Claim> query = _context.Claims
            .Include(c => c.Policy)
                .ThenInclude(p => p!.Customer)
            .Include(c => c.AssignedEmployee)
            .Include(c => c.Decision)
            .Include(c => c.GeneratedDocument)
            .Include(c => c.PolicyMatchResult)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.CustomerId))
            query = query.Where(c => c.Policy!.Customer!.Id == filter.CustomerId);

        if (!string.IsNullOrWhiteSpace(filter.EmployeeId))
            query = query.Where(c => c.AssignedEmployee!.Id == filter.EmployeeId);

        if (!string.IsNullOrWhiteSpace(filter.PolicyId))
            query = query.Where(c => c.Policy!.Id == filter.PolicyId);

        //Add ordering by customerId, and then by policyId
        query = query.OrderBy(c => c.Policy!.Customer!.Id)
                .ThenBy(c => c.Policy!.Id);

        return await query.ToListAsync();
    }

    public async Task UpdateAsync(Claim claim)
    {
        _context.Claims.Update(claim); 
        await _context.SaveChangesAsync();
    }
}
