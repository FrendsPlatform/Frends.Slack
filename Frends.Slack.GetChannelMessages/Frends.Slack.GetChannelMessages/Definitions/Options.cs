using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Slack.GetChannelMessages.Definitions;

/// <summary>
/// Additional parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// Whether to include thread replies
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool IncludeThreads { get; set; } = false;

    /// <summary>
    /// Whether to throw an error on failure.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; }

    /// <summary>
    /// Overrides the error message on failure.
    /// </summary>
    /// <example>Custom error message</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; }
}
