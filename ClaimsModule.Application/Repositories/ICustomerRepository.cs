using ClaimsModule.Domain.Entities;
using System.Threading.Tasks;

namespace ClaimsModule.Application.Repositories;

/// <summary>
/// Provides data access operations for accessing customer data from the persistence layer.
/// </summary>
public interface ICustomerRepository
{
    /// <summary>
    /// Retrieves a customer entity by its unique identifier.
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer.</param>
    /// <returns>The <see cref="Customer"/> if found; otherwise, null.</returns>
    Task<Customer?> GetByIdAsync(string customerId);
}
