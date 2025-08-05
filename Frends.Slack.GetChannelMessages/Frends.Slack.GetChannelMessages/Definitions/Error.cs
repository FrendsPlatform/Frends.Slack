namespace Frends.Slack.GetChannelMessages.Definitions;

/// <summary>
/// Error that occurred during the task.
/// </summary>
public class Error
{
    /// <summary>
    /// Summary of the error.
    /// </summary>
    /// <example>Unable to retrieve messages.</example>
    public string Message { get; set; }

    /// <summary>
    /// Additional information about the error.
    /// </summary>
    /// <example>ArgumentException</example>
    public string AdditionalInfo { get; set; }
}
