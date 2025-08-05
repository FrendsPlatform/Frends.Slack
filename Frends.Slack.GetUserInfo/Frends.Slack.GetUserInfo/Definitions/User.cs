using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frends.Slack.GetUserInfo.Definitions
{
    /// <summary>
    /// Represents a Slack user and their basic profile information.
    /// </summary>
    public class User
    {
        /// <summary>
        /// The unique identifier of the Slack user.
        /// </summary>
        /// <example>U012A3CDE</example>
        public string Id { get; set; }

        /// <summary>
        /// The Slack username (display name) of the user.
        /// </summary>
        /// <example>john.doe</example>
        public string Name { get; set; }

        /// <summary>
        /// The full name (real name) of the user as set in their Slack profile.
        /// </summary>
        /// <example>John Doe</example>
        public string RealName { get; set; }

        /// <summary>
        /// Indicates whether the user is a workspace administrator.
        /// </summary>
        /// <example>true</example>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// The email address associated with the user's Slack profile.
        /// </summary>
        /// <example>john.doe@example.com</example>
        public string Email { get; set; }
    }
}
