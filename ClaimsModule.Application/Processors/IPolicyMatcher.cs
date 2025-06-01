using ClaimsModule.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClaimsModule.Application.Processors;

/// <summary>
/// Defines the contract for matching a claim's description against a policy
/// using semantic or vector-based similarity.
/// </summary>
public interface IPolicyMatcher
{
    /// <summary>
    /// Computes the semantic similarity between a claim and the policy terms.
    /// </summary>
    /// <param name="claim">The claim to be analyzed.</param>
    /// <returns>A <see cref="PolicyMatchResult"/> containing the match score and details.</returns>
    Task<PolicyMatchResult> MatchAsync(Claim claim);
}
