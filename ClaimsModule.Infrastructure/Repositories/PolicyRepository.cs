using ClaimsModule.Application.Repositories;
using ClaimsModule.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClaimsModule.Infrastructure.Repositories;

/// <summary>
/// Entity Framework-based implementation of <see cref="IPolicyRepository"/>.
/// </summary>
public class PolicyRepository : IPolicyRepository
{
    private readonly ClaimsDbContext _context;

    public PolicyRepository(ClaimsDbContext context)
    {
        _context = context;
    }

    public async Task<Policy?> GetByIdAsync(string id)
    {
        return await _context.Policies.FindAsync(id);
    }
}