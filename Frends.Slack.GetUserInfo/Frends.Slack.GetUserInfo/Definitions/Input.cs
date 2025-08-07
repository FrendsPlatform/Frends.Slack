using System.ComponentModel.DataAnnotations;

namespace Frends.Slack.GetUserInfo.Definitions;

/// <summary>
/// Essential parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// The unique Slack user ID of the person whose information you want to retrieve.
    /// </summary>
    /// <example>U0A1BC2DE</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string UserId { get; set; }
}
