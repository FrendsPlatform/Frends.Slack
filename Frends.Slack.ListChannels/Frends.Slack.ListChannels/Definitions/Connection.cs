using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Slack.ListChannels.Definitions;

/// <summary>
/// Connection parameters.
/// </summary>
public class Connection
{
    /// <summary>
    /// Slack OAuth token used for authentication.
    /// If the token is a <b>bot token</b> (starts with "xoxb"), messages
    /// will be updated as the Slack app's bot user.
    /// If the token is a <b>user token</b> (starts with "xoxp"), messages
    /// will be updated as the Slack user who authorized the token.
    /// The token owner must be the original message poster
    /// or have appropriate permissions to edit the message.
    /// Note: User tokens require the user to have authorized the app via OAuth
    /// with appropriate scopes (e.g., "chat:write").
    /// </summary>
    /// <example>xoxb-123456789</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string Token { get; set; }
}
