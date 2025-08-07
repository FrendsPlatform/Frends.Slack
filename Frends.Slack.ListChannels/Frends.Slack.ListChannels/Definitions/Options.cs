using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Slack.ListChannels.Definitions;

/// <summary>
/// Additional parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// Whether to omit archived channels.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ExcludeArchived { get; set; } = true;

    /// <summary>
    /// Number of channels to return.
    /// </summary>
    /// <example>100</example>
    [DefaultValue(100)]
    public int Limit { get; set; } = 100;

    /// <summary>
    /// A pagination cursor used to continue listing channels from where the previous ListChannels task execution stopped.
    /// Set this to the NextCursor value returned in the previous result to fetch the next batch of channels.
    /// </summary>
    /// <example>e3VzZXI6...</example>
    public string Cursor { get; set; }

    /// <summary>
    /// Whether to throw an error on failure.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Overrides the error message on failure.
    /// </summary>
    /// <example>Custom error message</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; }
}
