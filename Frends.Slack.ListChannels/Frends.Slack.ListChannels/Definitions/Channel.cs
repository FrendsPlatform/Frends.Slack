namespace Frends.Slack.ListChannels.Definitions;

/// <summary>
/// Represents a single Slack channel, including its unique identifier, name, and archived status.
/// </summary>
public class Channel
{
    /// <summary>
    /// The unique identifier of the channel.
    /// </summary>
    /// <example>C01234567</example>
    public string Id { get; set; }

    /// <summary>
    /// The name of the channel.
    /// </summary>
    /// <example>general</example>
    public string Name { get; set; }

    /// <summary>
    /// Indicates whether the channel is archived.
    /// </summary>
    /// <example>false</example>
    public bool IsArchived { get; set; }
}
