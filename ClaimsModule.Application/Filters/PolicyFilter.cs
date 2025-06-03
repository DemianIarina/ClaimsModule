using System;

namespace ClaimsModule.Application.Filters;

/// <summary>
/// Represents filtering criteria used to query insurance policies.
/// This can be extended to support additional filters such as status or vehicle type.
/// </summary>
public class PolicyFilter
{
    /// <summary>
    /// Filters policies by the associated customer's unique identifier.
    /// </summary>
    public string? CustomerId { get; set; }

    /// <summary>
    /// Filters policies that are valid starting from this date.
    /// </summary>
    public DateTime? ValidFrom { get; set; }

    /// <summary>
    /// Filters policies that are valid up to this date.
    /// </summary>
    public DateTime? ValidTo { get; set; }
}
