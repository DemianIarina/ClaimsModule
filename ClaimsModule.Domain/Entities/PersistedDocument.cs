using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClaimsModule.Domain.Entities;

/// <summary>
/// Represents a file stored in persistent storage, such as a generated report or uploaded photo.
/// </summary>
public class PersistedDocument
{
    /// <summary>
    /// Unique identifier of the document.
    /// </summary>
    public string? Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// URL for accessing the file.
    /// </summary>
    [NotMapped]
    public string? FileUrl { get; set; }

    /// <summary>
    /// Timestamp of when the document was stored.
    /// </summary>
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Original name of the file (optional, useful for display).
    /// </summary>
    public string? OriginalFileName { get; set; }

    /// <summary>
    /// Generated name of the file.
    /// </summary>
    public string? GeneratedFileName { get; set; }

    /// <summary>
    /// MIME type of the file (e.g., image/png).
    /// </summary>
    public string? ContentType { get; set; }
}
