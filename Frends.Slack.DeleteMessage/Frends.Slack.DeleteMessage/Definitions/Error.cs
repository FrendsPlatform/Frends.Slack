using System;

namespace Frends.Slack.DeleteMessage.Definitions;

/// <summary>
/// Error that occurred during the task.
/// </summary>
public class Error
{
    /// <summary>
    /// Summary of the error.
    /// </summary>
    /// <example>Unable to join strings.</example>
    public string Message { get; set; }

    /// <summary>
    /// Additional information about the error.
    /// </summary>
    /// <example>new Exception("Slack API returned 400 Bad Request")</example>
    public Exception AdditionalInfo { get; set; }
}
