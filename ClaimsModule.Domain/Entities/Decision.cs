using ClaimsModule.Domain.Enums;
using System;
using System.Linq;

namespace ClaimsModule.Domain.Entities;

/// <summary>
/// Represents a decision made for a submitted claim.
/// </summary>
public class Decision
{
    private string? _type;

    /// <summary>
    /// Unique identifier of the decision.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Optional explanation for the decision.
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Who is responsible for the decision
    /// </summary>
    public string? DecidedBy { get; set; }

    /// <summary>
    /// Date and time when the decision was made.
    /// </summary>
    public DateTime DecidedAt { get; set; }

    /// <summary>
    /// Current status of the claim.
    /// Must have one of the values of <see cref="DecisionType"/>
    /// </summary>
    public string? Type
    {
        get => _type;
        set
        {
            if (!DecisionType.All.Contains(value))
                throw new ArgumentException($"Invalid decision type: {value}");

            _type = value;
        }
    }
}

