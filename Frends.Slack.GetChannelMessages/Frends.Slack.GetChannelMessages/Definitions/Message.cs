using System;

namespace Frends.Slack.GetChannelMessages.Definitions
{
    /// <summary>
    /// Represents a single Slack message, including its text content, timestamp, and the ID of the user who posted it.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// The text content of the message
        /// </summary>
        /// <example>Hello world!</example>
        public string Text { get; set; }

        /// <summary>
        /// Timestamp of the message
        /// </summary>
        /// <example>7/31/2025 10:04:07 AM</example>
        public DateTime? Ts { get; set; }

        /// <summary>
        /// ID of the user who posted the message
        /// </summary>
        /// <example>U12345678</example>
        public string User { get; set; }
    }
}
