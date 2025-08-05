using System;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Frends.Slack.GetChannelMessages.Definitions;
using Frends.Slack.GetUserInfo.Definitions;
using Frends.Slack.GetUserInfo.Helpers;
using Newtonsoft.Json.Linq;

namespace Frends.Slack.GetUserInfo;

/// <summary>
/// Task class.
/// </summary>
public static class Slack
{
    /// <summary>
    /// Task to retrieve information about a Slack user.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends-Slack-GetUserInfo)
    /// </summary>
    /// <param name="input">Input parameters including the Slack UserId.</param>
    /// <param name="connection">Slack connection settings including the OAuth token.</param>
    /// <param name="options">Optional settings such as error handling flags (e.g., ThrowErrorOnFailure and ErrorMessageOnFailure).</param>
    /// <param name="cancellationToken">A cancellation token provided by the Frends Platform.</param>
    /// <returns>
    /// Result {
    ///     bool Success,
    ///     User User
    ///     {
    ///       "Id": "U012A3CDE",
    ///       "Name": "john.doe",
    ///       "RealName": "John Doe",
    ///       "IsAdmin": true,
    ///       "Email": "john.doe@example.com"
    ///     },
    ///     Error Error { string Message, object AdditionalInfo }
    /// }
    /// </returns>
    public static async Task<Result> GetUserInfo(
        [PropertyTab] Input input,
        [PropertyTab] Connection connection,
        [PropertyTab] Options options,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(connection.Token))
                throw new ArgumentException("Slack token is required", nameof(connection.Token));

            if (string.IsNullOrWhiteSpace(input.UserId))
                throw new ArgumentException("User ID is required", nameof(input.UserId));

            var requestUrl = $"https://slack.com/api/users.info?user={Uri.EscapeDataString(input.UserId)}";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", connection.Token);

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
                new Exception($"Slack API error: {error}", innerException),
                options.ThrowErrorOnFailure,
                options.ErrorMessageOnFailure);
            }

            var userToken = json["user"];
            var profile = userToken?["profile"];

            var user = new User
            {
                Id = userToken?["id"]?.Value<string>(),
                Name = userToken?["name"]?.Value<string>(),
                RealName = userToken?["real_name"]?.Value<string>(),
                IsAdmin = userToken?["is_admin"]?.Value<bool>() ?? false,
                Email = profile?["email"]?.Value<string>(),
            };

            return new Result
            {
                Success = true,
                User = user,
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
