using System.ComponentModel.DataAnnotations;

namespace Frends.Slack.GetChannelMessages.Definitions;

/// <summary>
/// Essential parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Channel ID where the message exists
    /// </summary>
    /// <example>C12345678</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string ChannelId { get; set; }

    /// <summary>
    /// Maximum number of messages to retrieve
    /// </summary>
    /// <example>100</example>
    [DisplayFormat(DataFormatString = "Number")]
    public int? Limit { get; set; }

    /// <summary>
    /// Fetch messages before this timestamp (used for pagination)
    /// </summary>
    /// <example>1623878400.000200</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string Latest { get; set; }

    /// <summary>
    /// Fetch messages after this timestamp (used for pagination)
    /// </summary>
    /// <example>1623792000.000100</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string Oldest { get; set; }
}
