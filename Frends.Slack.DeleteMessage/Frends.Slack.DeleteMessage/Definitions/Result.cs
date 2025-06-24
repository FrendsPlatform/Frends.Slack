namespace Frends.Slack.DeleteMessage.Definitions;

/// <summary>
/// Result of the task.
/// </summary>
public class Result
{
    internal Result(string messageTs = null, bool success = true, Error error = null)
    {
        Success = success;
        MessageTs = messageTs;
        Error = error;
    }

    /// <summary>
    /// Indicates whether the message was deleted successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// The timestamp (ID) of the deleted message.
    /// </summary>
    /// <example>1234567890.123456</example>
    public string MessageTs { get; set; }

    /// <summary>
    /// Error that occurred during task execution.
    /// </summary>
    /// <example>{ Message = "Failed to delete Slack message.", AdditionalInfo = new Exception("Slack API returned 400 Bad Request") }</example>
    public Error Error { get; set; }
}
