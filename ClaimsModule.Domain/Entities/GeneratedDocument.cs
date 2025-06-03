using System;

namespace ClaimsModule.Domain.Entities;

/// <summary>
/// Represents a document generated for a claim.
/// </summary>
public class GeneratedDocument
{
    /// <summary>
    /// Unique identifier of the document.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Path to the stored file.
    /// </summary>
    public string? FileUrl { get; set; }

    /// <summary>
    /// Timestamp of when the document was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }
}
