using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClaimsModule.Infrastructure.Config;

/// <summary>
/// Holds configuration values for decision thresholds in the engine.
/// </summary>
public class DecisionThresholds
{
    /// <summary>
    /// Minimum similarity score for a claim to be approved.
    /// </summary>
    public float ApproveThreshold { get; set; } = 0.8f;

    /// <summary>
    /// Minimum similarity score for a claim to be escalated.
    /// Below this, it is rejected.
    /// </summary>
    public float EscalateThreshold { get; set; } = 0.5f;
}
