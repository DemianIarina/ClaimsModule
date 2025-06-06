namespace ClaimsModule.Infrastructure.Config;

/// <summary>
/// The configurable email templates used for various claim status notifications.
/// The templates support placeholders (e.g. {ClaimId}, {CustomerName}) that can be replaced dynamically at runtime.
/// </summary>
public class EmailTemplates
{
    /// <summary>
    /// Subject template for emails sent when a claim is approved.
    /// </summary>
    public string ClaimApprovedSubject { get; set; } = string.Empty;

    /// <summary>
    /// Body template for emails sent when a claim is approved.
    /// </summary>
    public string ClaimApprovedBody { get; set; } = string.Empty;

    /// <summary>
    /// Subject template for emails sent when a claim is escalated.
    /// </summary>
    public string ClaimEscalatedSubject { get; set; } = string.Empty;

    /// <summary>
    /// Body template for emails sent when a claim is escalated.
    /// </summary>
    public string ClaimEscalatedBody { get; set; } = string.Empty;

    /// <summary>
    /// Subject template for emails sent when a claim is rejected.
    /// </summary>
    public string ClaimRejectedSubject { get; set; } = string.Empty;

    /// <summary>
    /// Body template for emails sent when a claim is rejected.
    /// </summary>
    public string ClaimRejectedBody { get; set; } = string.Empty;

    /// <summary>
    /// Subject template for emails sent to an employee when an escalated claim is assigned for manual review.
    /// </summary>
    public string ClaimEscalationAssignedSubject { get; set; } = string.Empty;

    /// <summary>
    /// Body template for emails sent to an employee when an escalated claim is assigned for manual review.
    /// </summary>
    public string ClaimEscalationAssignedBody { get; set; } = string.Empty;
}
