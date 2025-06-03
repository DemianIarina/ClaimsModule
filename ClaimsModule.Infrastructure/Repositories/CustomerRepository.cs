using ClaimsModule.Application.Repositories;
using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ClaimsModule.Infrastructure.Repositories;

/// <summary>
/// Entity Framework implementation of <see cref="ICustomerRepository"/> for accessing customer data.
/// </summary>
public class CustomerRepository : ICustomerRepository
{
    private readonly ClaimsDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerRepository"/> class.
    /// </summary>
    /// <param name="context">The database context for customer storage.</param>
    public CustomerRepository(ClaimsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc/>
    public async Task<Customer?> GetByIdAsync(string customerId)
    {
        return await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == customerId);
    }
}