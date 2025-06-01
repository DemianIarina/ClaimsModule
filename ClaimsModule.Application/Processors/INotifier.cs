using ClaimsModule.Domain.Entities;
using System.Threading.Tasks;

namespace ClaimsModule.Application.Processors;

/// <summary>
/// Defines the contract for notifying stakeholders (clients or staff)
/// about the result of the claim decision.
/// </summary>
public interface INotifier
{
    /// <summary>
    /// Sends a notification based on the decision made for a claim.
    /// </summary>
    /// <param name="claim">The claim that was evaluated.</param>
    /// <param name="decision">The result of the claim evaluation.</param>
    Task NotifyAsync(Claim claim, Decision decision);
}
