using ClaimsModule.Application.Processors;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;
using System;

namespace ClaimsModule.Infrastructure.Processors;

public class ManualDecisionEngine : IDecisionEngine
{
    private readonly string _employeeId;
    private readonly bool _approved;

    public ManualDecisionEngine(string employeeId, bool approved)
    {
        _employeeId = employeeId;
        _approved = approved;
    }

    public Decision EvaluateClaim(Claim claim)
    {
        return new Decision
        {
            Id = Guid.NewGuid().ToString(),
            Reason = "Decision based on manual intervention",
            DecidedAt = DateTime.UtcNow,
            DecidedBy = _employeeId,
            Type = _approved ? DecisionType.Approved : DecisionType.Rejected
        };
    }
}
