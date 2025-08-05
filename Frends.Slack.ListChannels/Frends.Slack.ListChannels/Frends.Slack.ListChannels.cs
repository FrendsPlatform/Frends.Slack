using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Frends.Slack.ListChannels.Definitions;
using Frends.Slack.ListChannels.Helpers;
using Newtonsoft.Json.Linq;

namespace Frends.Slack.ListChannels;

/// <summary>
/// Task class.
/// </summary>
public static class Slack
{
    /// <summary>
    /// Task to list channels from a Slack workspace.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends-Slack-ListChannels)
    /// </summary>
    /// <param name="connection">Connection parameters.</param>
    /// <param name="options">Additional parameters.</param>
    /// <param name="cancellationToken">A cancellation token provided by Frends Platform.</param>
    /// <returns>
    /// Result {
    ///     bool Success,
    ///     Messages (list of Message),
    ///     bool HasMore,
    ///     Error Error { string Message, object AdditionalInfo }
    /// }
    /// </returns>
    public static async Task<Result> ListChannels(
    [PropertyTab] Connection connection,
    [PropertyTab] Options options,
    CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(connection.Token))
                throw new ArgumentException("Slack token is required", nameof(connection.Token));

            var channels = new List<Channel>();
            var cursor = options.Cursor ?? string.Empty;
            var hasMore = false;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", connection.Token);

            do
            {
                var queryParams = new Dictionary<string, string>
                {
                    ["exclude_archived"] = options.ExcludeArchived.ToString().ToLowerInvariant(),
                    ["limit"] = options.Limit.ToString(),
                };

                if (!string.IsNullOrEmpty(cursor))
                    queryParams["cursor"] = cursor;

                var requestUrl = "https://slack.com/api/conversations.list?" +
                                 string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

                var response = await client.GetAsync(requestUrl, cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var json = JObject.Parse(content);

                if (!response.IsSuccessStatusCode || !(json["ok"]?.Value<bool>() ?? false))
                {
                    var error = json["error"]?.Value<string>() ?? "Unknown error";
                    var needed = json["needed"]?.Value<string>();
                    var provided = json["provided"]?.Value<string>();

                    var innerDetails = $"HTTP {response.StatusCode}: {response.ReasonPhrase}";
                    if (!string.IsNullOrEmpty(needed))
                        innerDetails += $" | Needed: {needed}";
                    if (!string.IsNullOrEmpty(provided))
                        innerDetails += $" | Provided: {provided}";

                    var innerException = new HttpRequestException(innerDetails);

                    return ErrorHandler.Handle(
                        new Exception($"Slack API error: {error}"),
                        options.ThrowErrorOnFailure,
                        options.ErrorMessageOnFailure);
                }

                foreach (var ch in json["channels"] ?? Enumerable.Empty<JToken>())
                {
                    if (channels.Count >= options.Limit)
                    {
                        break;
                    }

                    channels.Add(new Channel
                    {
                        Id = ch["id"]?.Value<string>(),
                        Name = ch["name"]?.Value<string>(),
                        IsArchived = ch["is_archived"]?.Value<bool>() ?? false,
                    });
                }

                cursor = json["response_metadata"]?["next_cursor"]?.Value<string>() ?? string.Empty;
                hasMore = !string.IsNullOrEmpty(cursor);

                if (channels.Count >= options.Limit)
                    break;
            }
            while (hasMore);

            return new Result
            {
                Success = true,
                Channels = channels,
                HasMore = hasMore,
                NextCursor = hasMore ? cursor : null,
            };
        }
        catch (Exception ex)
        {
            return ErrorHandler.Handle(
                ex,
                options.ThrowErrorOnFailure,
                options.ErrorMessageOnFailure);
        }
    }
}
