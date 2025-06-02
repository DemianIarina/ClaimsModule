using ClaimsModule.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClaimsModule.Application.Processors;

/// <summary>
/// Defines the contract for making a decision on a claim based on business rules
/// and the policy match results.
/// </summary>
public interface IDecisionEngine
{
    /// <summary>
    /// Evaluates the claim and its policy match result to determine the next action.
    /// </summary>
    /// <param name="claim">The claim under review.</param>
    /// <returns>A <see cref="Decision"/> indicating approval, rejection, or escalation.</returns>
    Decision EvaluateClaim(Claim claim);
}
