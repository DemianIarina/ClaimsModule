using ClaimsModule.Domain.Enums;
using System;
using System.Globalization;
using System.Linq;

namespace ClaimsModule.Domain.Entities;

/// <summary>
/// Represents the semantic similarity result between a claim and a policy.
/// </summary>
public class PolicyMatchResult
{
    /// <summary>
    /// Unique identifier for the match result.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Semantic similarity score (0.0000 to 1.0000).
    /// </summary>
    public float? SimilarityScore { get; set; }

    /// <summary>
    /// Timestamp of when the match was generated.
    /// </summary>
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
}
