using ClaimsModule.Application.Processors;
using ClaimsModule.Application.Repositories;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ClaimsModule.Infrastructure.Processors;

/// <summary>
/// Implements claim decision logic based on configurable policy match score thresholds.
/// </summary>
public class RuleBasedDecisionEngine : IDecisionEngine
{
    private readonly ILogger<RuleBasedDecisionEngine> _logger;

    private readonly IClaimRepository _claimRepository;

    public RuleBasedDecisionEngine(ILogger<RuleBasedDecisionEngine> logger, IClaimRepository claimRepository)
    {
        _logger = logger;

        _claimRepository = claimRepository;
    }

    /// <inheritdoc />
    public async Task<Decision> EvaluateAsync(Claim claim)
    {
        float? score = claim.PolicyMatchResult!.SimilarityScore;

        _logger.LogInformation("Evaluating claim {ClaimId} with match score {Score}", claim.Id, score);

        Decision metDecision = new(); 

        if (score >= 0.8)
            metDecision.Type = DecisionType.Approved;
        else if (score >= 0.5)
            metDecision.Type = DecisionType.Escalated;
        else
            metDecision.Type = DecisionType.Rejected;

        var decision = new Decision
        {
            Id = Guid.NewGuid().ToString(),
            Reason = $"Decision based on match score {score:F2}",
            DecidedAt = DateTime.UtcNow,
            DecidedBy = "AutoEngine"
        };
        _logger.LogInformation("Successfully evaluated claim {ClaimId} to decision {DecisionType}", claim.Id, decision.Type);

        claim.Decision = metDecision;
        try
        {
            await _claimRepository.UpdateAsync(claim);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not save decision {Decision} for claim {ClaimId}", decision, claim.Id);
            throw;
        }

        _logger.LogInformation("Updated Claim {ClaimId} with the evaluated Decision {Decision}", claim.Id, decision.Type);

        return decision;
    }
}
