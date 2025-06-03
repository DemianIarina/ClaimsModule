using ClaimsModule.Application.Filters;
using ClaimsModule.Application.Repositories;
using ClaimsModule.Application.Services;
using ClaimsModule.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClaimsModule.Infrastructure.Services;

public class PolicyService : IPolicyService
{
    private readonly ILogger<PolicyService> _logger;
    private readonly ICustomerRepository _customerRepository;
    private readonly IPolicyRepository _policyRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="PolicyService"/> class.
    /// </summary>
    /// <param name="customerRepository">The repository used to access customer data.</param>
    /// <param name="policyRepository">The repository used to access policy data.</param>
    /// <param name="logger"><see cref="ILogger"/></param>
    public PolicyService(ICustomerRepository customerRepository, IPolicyRepository policyRepository, ILogger<PolicyService> logger)
    {
        _customerRepository = customerRepository;
        _policyRepository = policyRepository;
        _logger = logger;
    }

    public async Task<(Customer, List<Policy>)> GetPoliciesForCustomerAsync(string customerId)
    {
        _logger.LogTrace("Fetching customer data for ID: {CustomerId}", customerId);

        Customer? customer = await _customerRepository.GetByIdAsync(customerId);

        if (customer == null)
        {
            _logger.LogDebug("Customer not found for ID: {CustomerId}", customerId);
            throw new KeyNotFoundException($"Customer {customerId} not found");
        }

        _logger.LogTrace("Successfully fetched customer data {Customer} for ID {CustomerID}", customer, customerId);

        _logger.LogTrace("Fetching policies for customer with ID: {CustomerId}", customerId);

        PolicyFilter filter = new()
        {
            CustomerId = customerId
        };

        List<Policy> policies = await _policyRepository.GetListAsync(filter);

        _logger.LogTrace("Found {PolicyCount} policies for customer {CustomerId}", policies.Count, customerId);

        return (customer, policies);
    }
}
