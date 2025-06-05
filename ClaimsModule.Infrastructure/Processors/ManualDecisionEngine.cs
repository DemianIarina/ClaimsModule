using ClaimsModule.Application.Processors;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;
using System;

namespace ClaimsModule.Infrastructure.Processors;

public class ManualDecisionEngine : IDecisionEngine
{
    private readonly string _employeeName;
    private readonly bool _approved;

    public ManualDecisionEngine(string employeeName, bool approved)
    {
        _employeeName = employeeName;
        _approved = approved;
    }

    public Decision EvaluateClaim(Claim claim)
    {
        return new Decision
        {
            Id = Guid.NewGuid().ToString(),
            Reason = "Decision based on manual intervention",
            DecidedAt = DateTime.UtcNow,
            DecidedBy = _employeeName,
            Type = _approved ? DecisionType.Approved : DecisionType.Rejected
        };
    }
}
