using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClaimsModule.Domain.Entities;

/// <summary>
/// Represents an insurance policy that can be associated with a claim.
/// Each policy belongs to a single client, and a client may own multiple policies.
/// </summary>
public class Policy
{
    /// <summary>
    /// The unique identifier of the policy.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The identifier of the client who owns this policy.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;
}

