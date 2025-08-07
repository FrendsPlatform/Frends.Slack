using System.Collections.Generic;

namespace Frends.Slack.ListChannels.Definitions;

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
    /// List of channels retrieved from the Slack workspace. Each channel includes its ID, name, and archive status.
    /// </summary>
    /// <example>
    /// {
    /// "Id": "C01234567",
    /// "Name": "general",
    /// "IsArchived": true
    /// }
    /// </example>
    public List<Channel> Channels { get; set; }

    /// <summary>
    /// Indicates whether more channels are available for pagination. If true, additional requests using a cursor may return more channels.
    /// </summary>
    /// <example>true</example>
    public bool HasMore { get; set; }

    /// <summary>
    /// A pagination token to continue listing more channels. Use this value in the next execution of the ListChannels task to continue retrieving channels from where the previous execution ended.
    /// </summary>
    /// <example>dGVhbTpD...</example>
    public string NextCursor { get; set; }

    /// <summary>
    /// Error that occurred during task execution.
    /// </summary>
    /// <example>object { string Message, object { Exception Exception } AdditionalInfo }</example>
    public Error Error { get; set; }
}
