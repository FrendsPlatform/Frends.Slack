namespace Frends.Slack.GetUserInfo.Definitions;

/// <summary>
/// Result of the task.
/// </summary>
public class Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="success">Indicates whether the operation was successful.</param>
    /// <param name="error">Details of the error, if the operation failed.</param>
    internal Result(bool success = true, Error error = null)
    {
        Success = success;
        Error = error;
    }

    /// <summary>
    /// Indicates if the task completed successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// The Slack user information retrieved by the task.
    /// </summary>
    /// <example>
    /// {
    ///   "Id": "U012A3CDE",
    ///   "Name": "john.doe",
    ///   "RealName": "John Doe",
    ///   "IsAdmin": true,
    ///   "Email": "john.doe@example.com"
    /// }
    /// </example>
    public User User { get; set; }

    /// <summary>
    /// Error that occurred during task execution.
    /// </summary>
    /// <example>object { string Message, object { Exception Exception } AdditionalInfo }</example>
    public Error Error { get; set; }
}
