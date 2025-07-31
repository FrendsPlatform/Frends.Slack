using System;

namespace Frends.Slack.GetChannelMessages.Helpers;

/// <summary>
/// Helper class for utility methods related to Slack message processing.
/// </summary>
internal class Helper
{
    /// <summary>
    /// Converts a Slack timestamp string (Unix format, possibly with fractional seconds) to a UTC DateTime.
    /// </summary>
    /// <param name="ts">The Slack timestamp string (e.g., "1722320647.269739").</param>
    /// <returns>A UTC DateTime if conversion succeeds; otherwise, null.</returns>
    internal static DateTime? SlackTimestampToDateTime(string ts)
    {
        if (double.TryParse(ts, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double unixTime))
        {
            // Extract integer seconds from fractional Unix timestamp
            var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds((long)unixTime);
            return dateTimeOffset.UtcDateTime;
        }

        return null;
    }
}