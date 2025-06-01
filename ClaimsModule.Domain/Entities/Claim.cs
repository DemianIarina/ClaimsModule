using ClaimsModule.Domain.Enums;
using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace ClaimsModule.Domain.Entities;

/// <summary>
/// Represents a customer's claim for damage or loss.
/// </summary>
public class Claim
{
    private string? _status;

    /// <summary>
    /// Unique identifier of the claim.
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Identifier of the customer who submitted the claim.
    /// </summary>
    public string? CustomerId { get; set; }

    /// <summary>
    /// Identifier of the matched insurance policy.
    /// </summary>
    public string? PolicyId { get; set; }

    /// <summary>
    /// Free-text description of the damage or event.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Timestamp when the claim was submitted.
    /// </summary>
    public DateTime? SubmittedAt { get; set; }

    /// <summary>
    /// Current status of the claim.
    /// Must have one of the values of <see cref="ClaimStatus"/>
    /// </summary>
    public string? Status
    {
        get => _status;
        set
        {
            if (!ClaimStatus.All.Contains(value))
                throw new ArgumentException($"Invalid policy match status: {value}");

            _status = value;
        }
    }

    /// <summary>
    /// The result of the match of the claim agains the corresponding policy of the customer.
    /// </summary>
    public PolicyMatchResult? PolicyMatchResult { get; set; }

    /// <summary>
    /// The Document generated for a claim.
    /// </summary>
    public GeneratedDocument? GeneratedDocument { get; set; }

    /// <summary>
    /// The <see cref="Decision"/> made for the claim after automatic review.
    /// </summary>
    public Decision? Decision { get; set; }
}
