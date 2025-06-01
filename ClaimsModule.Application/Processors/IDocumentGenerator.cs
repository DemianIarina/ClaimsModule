using ClaimsModule.Domain.Entities;
using System.Threading.Tasks;

namespace ClaimsModule.Application.Processors;

/// <summary>
/// Defines the contract for generating official documents related to a claim,
/// such as a repair authorization.
/// </summary>
public interface IDocumentGenerator
{
    /// <summary>
    /// Generates a document based on the approved claim.
    /// </summary>
    /// <param name="claim">The approved claim to generate a document for.</param>
    /// <returns>A <see cref="GeneratedDocument"/> with file URL and metadata.</returns>
    Task<GeneratedDocument> GenerateAsync(Claim claim);
}
