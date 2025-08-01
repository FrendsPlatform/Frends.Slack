using System;

namespace Frends.Slack.GetChannelMessages.Helpers;

/// <summary>
/// Helper class for utility methods related to Slack message processing.
/// </summary>
internal class Helper
{
    /// <summary>
    /// Converts a Slack timestamp string (Unix format with fractional seconds) to a UTC DateTime with millisecond precision.
    /// </summary>
    /// <param name="ts">The Slack timestamp string (e.g., "1722320647.269739").</param>
    /// <returns>A UTC DateTime if conversion succeeds; otherwise, null.</returns>
    internal static DateTime? SlackTimestampToDateTime(string ts)
    {
        if (double.TryParse(ts, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double unixTime))
        {
            var dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds((long)(unixTime * 1000));
            return dateTimeOffset.UtcDateTime;
        }

        return null;
    }
}