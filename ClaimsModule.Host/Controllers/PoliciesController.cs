using ClaimsModule.Application.Services;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Host.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClaimsModule.Host.Controllers;

/// <summary>
/// Handles requests related to retrieving insurance policies for a specific customer.
/// </summary>
[ApiController]
[Route("policies")]
[Authorize(Policy = "CustomerOnly")]
public class PoliciesController : ControllerBase
{
    private readonly IPolicyService _policyService;

    public PoliciesController(IPolicyService policyService)
    {
        _policyService = policyService;
    }

    /// <summary>
    /// Retrieves a customer along with all associated insurance policies.
    /// </summary>
    /// <param name="customerId">The ID of the customer whose policies are to be retrieved.</param>
    /// <returns>A composite object containing customer information and associated policies.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetPoliciesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<GetPoliciesResponse>>> GetPoliciesForCustomer()
    {
        string? customerId = User.Claims.FirstOrDefault(c => c.Type == "CustomerId")?.Value;

        if (string.IsNullOrEmpty(customerId))
        {
            return Unauthorized("No customer ID found in the token.");
        }

        Customer customer;
        List<Policy> policies;

        try
        {
            (customer, policies) = await _policyService.GetPoliciesForCustomerAsync(customerId);
        }
        catch(KeyNotFoundException)
        {
            return NotFound($"Could not find customer with id {customerId}");
        }

        GetPoliciesResponse response = new()
        {
            Customer = new CustomerDto
            {
                Id = customer.Id,
                FullName = customer.Name,
                Email = customer.Email
            },
            Policies = policies.Select(p => new PolicyDto
            {
                Id = p.Id,
                PolicyNumber = p.PolicyNumber,
                CarPlateNumber = p.CarPlateNumber,
                CarBrand = p.CarBrand,
                CarModel = p.CarModel,
                ValidFrom = p.ValidFrom,
                ValidTo = p.ValidTo,
                DocumentReference = p.PolicyDocument!.FileUrl
            }).ToList()
        };

        return Ok(response);
    }
}

