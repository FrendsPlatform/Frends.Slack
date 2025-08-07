using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Frends.Slack.GetChannelMessages.Definitions;
using Frends.Slack.GetChannelMessages.Helpers;
using Newtonsoft.Json.Linq;

namespace Frends.Slack.GetChannelMessages;

/// <summary>
/// Task class.
/// </summary>
public static class Slack
{
    /// <summary>
    /// Task to retrieve recent messages from a Slack channel, with optional parameters for pagination and thread inclusion.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends-Slack-GetChannelMessages)
    /// </summary>
    /// <param name="input">Input parameters including ChannelId, Limit, Oldest, and Latest timestamps.</param>
    /// <param name="connection">Slack connection settings including OAuth token.</param>
    /// <param name="options">Optional parameters such as thread inclusion.</param>
    /// <param name="cancellationToken">A cancellation token provided by Frends Platform.</param>
    /// <returns>
    /// Result {
    ///     bool Success,
    ///     Messages (list of Message),
    ///     bool HasMore,
    ///     Error Error { string Message, object AdditionalInfo }
    /// }
    /// </returns>
    public static async Task<Result> GetChannelMessages(
        [PropertyTab] Input input,
        [PropertyTab] Connection connection,
        [PropertyTab] Options options,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(connection.Token))
                throw new ArgumentException("Slack token is required", nameof(connection.Token));

            if (string.IsNullOrWhiteSpace(input.ChannelId))
                throw new ArgumentException("Channel ID is required", nameof(input.ChannelId));

            var queryParams = new Dictionary<string, string>
            {
                ["channel"] = input.ChannelId,
            };

            if (input.Limit.HasValue)
                queryParams["limit"] = input.Limit.Value.ToString();

            if (!string.IsNullOrWhiteSpace(input.Latest))
                queryParams["latest"] = input.Latest;

            if (!string.IsNullOrWhiteSpace(input.Oldest))
                queryParams["oldest"] = input.Oldest;

            var requestUrl = "https://slack.com/api/conversations.history?" +
                             string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", connection.Token);

            var response = await client.GetAsync(requestUrl, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var json = JObject.Parse(content);

            if (!response.IsSuccessStatusCode || !(json["ok"]?.Value<bool>() ?? false))
            {
                var error = json["error"]?.Value<string>() ?? "Unknown error";
                throw new Exception($"Slack API error: {error}");
            }

            var messages = new List<Message>();

            foreach (var msg in json["messages"])
            {
                var mainMessage = new Message
                {
                    Text = msg["text"]?.Value<string>(),
                    Ts = msg["ts"] != null ? Helper.SlackTimestampToDateTime(msg["ts"].Value<string>()) : (DateTime?)null,
                    User = msg["user"]?.Value<string>(),
                };
                messages.Add(mainMessage);

                if (options.IncludeThreads && msg["thread_ts"] != null && msg["thread_ts"].Value<string>() == msg["ts"].Value<string>())
                {
                    var threadTs = msg["thread_ts"].Value<string>();
                    var threadUrl = $"https://slack.com/api/conversations.replies?channel={Uri.EscapeDataString(input.ChannelId)}&ts={Uri.EscapeDataString(threadTs)}";

                    var threadResponse = await client.GetAsync(threadUrl, cancellationToken);
                    var threadContent = await threadResponse.Content.ReadAsStringAsync(cancellationToken);
                    var threadJson = JObject.Parse(threadContent);

                    if (threadResponse.IsSuccessStatusCode && (threadJson["ok"]?.Value<bool>() ?? false))
                    {
                        foreach (var reply in threadJson["messages"])
                        {
                            if (reply["ts"]?.Value<string>() != msg["ts"]?.Value<string>())
                            {
                                messages.Add(new Message
                                {
                                    Text = reply["text"]?.Value<string>(),
                                    Ts = reply["ts"] != null ? Helper.SlackTimestampToDateTime(reply["ts"].Value<string>()) : (DateTime?)null,
                                    User = reply["user"]?.Value<string>(),
                                });
                            }
                        }
                    }
                }
            }

            return new Result
            {
                Success = true,
                Messages = messages,
                HasMore = json["has_more"]?.Value<bool>() ?? false,
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
