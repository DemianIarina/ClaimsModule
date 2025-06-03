using ClaimsModule.Application.Filters;
using ClaimsModule.Application.Repositories;
using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClaimsModule.Infrastructure.Repositories;

/// <summary>
/// Entity Framework-based implementation of <see cref="IPolicyRepository"/>.
/// </summary>
public class PolicyRepository : IPolicyRepository
{
    private readonly ClaimsDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="PolicyRepository"/> class.
    /// </summary>
    /// <param name="context">The database context for policy storage.</param>
    public PolicyRepository(ClaimsDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<Policy?> GetByIdAsync(string id)
    {
        return await _context.Policies.FindAsync(id);
    }

    /// <inheritdoc/>
    public async Task<List<Policy>> GetListAsync(PolicyFilter filter)
    {
        IQueryable<Policy> query = _context.Policies.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.CustomerId))
            query = query.Where(p => p.Customer!.Id == filter.CustomerId);

        if (filter.ValidFrom.HasValue)
            query = query.Where(p => p.ValidFrom >= filter.ValidFrom.Value);

        if (filter.ValidTo.HasValue)
            query = query.Where(p => p.ValidTo <= filter.ValidTo.Value);

        return await query.ToListAsync();
    }
}