using ClaimsModule.Application.Processors;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;
using ClaimsModule.Infrastructure.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace ClaimsModule.Infrastructure.Processors;

/// <summary>
/// Implements claim decision logic based on configurable policy match score thresholds.
/// </summary>
public class RuleBasedDecisionEngine : IDecisionEngine
{
    private readonly ILogger<RuleBasedDecisionEngine> _logger;

    private readonly DecisionThresholds _thresholds;

    public RuleBasedDecisionEngine(ILogger<RuleBasedDecisionEngine> logger, IOptions<DecisionThresholds> thresholds)
    {
        _logger = logger;

        _thresholds = thresholds.Value;
    }

    /// <inheritdoc />
    public Decision EvaluateClaim(Claim claim)
    {
        float? score = claim.PolicyMatchResult!.SimilarityScore;

        _logger.LogInformation("Evaluating claim {ClaimId} with match score {Score}", claim.Id, score);

        Decision metDecision = new Decision
        {
            Id = Guid.NewGuid().ToString(),
            Reason = $"Decision based on match score {score:F2}",
            DecidedAt = DateTime.UtcNow,
            DecidedBy = "AutoEngine"
        };

        // set the type based on the score
        if (score >= _thresholds.ApproveThreshold)
            metDecision.Type = DecisionType.Approved;
        else if (score >= _thresholds.EscalateThreshold)
            metDecision.Type = DecisionType.Escalated;
        else
            metDecision.Type = DecisionType.Rejected;

        _logger.LogInformation("Successfully evaluated claim {ClaimId} to decision {DecisionType}", claim.Id, metDecision.Type);

        return metDecision;
    }
}
