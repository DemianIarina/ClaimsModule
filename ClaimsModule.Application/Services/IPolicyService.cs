using ClaimsModule.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClaimsModule.Application.Services;

/// <summary>
/// Defines operations for handling the policies of a customer.
/// </summary>
public interface IPolicyService
{
    /// <summary>
    /// Retrieves all insurance policies for a specific customer, including customer details.
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer.</param>
    /// <returns>
    /// A tuple containing the <see cref="Customer"/> and a list of associated <see cref="Policy"/> entities.
    /// </returns>
    Task<(Customer,List<Policy>)> GetPoliciesForCustomerAsync(string customerId);
}
