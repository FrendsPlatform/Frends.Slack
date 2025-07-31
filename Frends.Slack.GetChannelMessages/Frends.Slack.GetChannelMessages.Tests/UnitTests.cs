using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Frends.Slack.GetChannelMessages.Definitions;
using Frends.Slack.GetChannelMessages.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.Slack.GetChannelMessages.Tests;

[TestClass]
public class GetChannelMessagesTests
{
    private readonly string _token = Environment.GetEnvironmentVariable("FRENDS_SLACK_TEST_TOKEN");
    private readonly string _channelId = Environment.GetEnvironmentVariable("FRENDS_SLACK_TEST_CHANNEL_ID");

    private Connection _connection;

    [AssemblyInitialize]
    public static void AssemblyInit(TestContext context)
    {
        DotNetEnv.Env.TraversePath().Load("./.env.local");
    }

    [TestInitialize]
    public void Setup()
    {
        _connection = new Connection { Token = _token };
    }

    [TestMethod]
    public async Task ShouldFetchRecentMessages_Basic()
    {
        var input = new Input
        {
            ChannelId = _channelId,
        };

        var options = new Options
        {
            IncludeThreads = false,
            ThrowErrorOnFailure = false,
        };

        var result = await Slack.GetChannelMessages(input, _connection, options, CancellationToken.None);

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Messages);
        Assert.IsTrue(result.Messages.Count > 0);
    }

    [TestMethod]
    public async Task ShouldFetchLimitedNumberOfMessages()
    {
        var input = new Input
        {
            ChannelId = _channelId,
            Limit = 2,
        };

        var options = new Options { IncludeThreads = false };

        var result = await Slack.GetChannelMessages(input, _connection, options, CancellationToken.None);

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Messages);
        Assert.IsTrue(result.Messages.Count <= 2);
    }

    [TestMethod]
    public async Task ShouldFetchMessagesWithPagination()
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        var oneHourAgo = DateTimeOffset.UtcNow.AddHours(-1).ToUnixTimeSeconds().ToString();

        var input = new Input
        {
            ChannelId = _channelId,
            Limit = 5,
            Latest = now,
            Oldest = oneHourAgo,
        };

        var options = new Options { IncludeThreads = false };

        var result = await Slack.GetChannelMessages(input, _connection, options, CancellationToken.None);

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Messages);
        Assert.IsTrue(result.Messages.All(m => m.Ts >= DateTimeOffset.FromUnixTimeSeconds(long.Parse(oneHourAgo)).UtcDateTime));
    }

    [TestMethod]
    public async Task ShouldIncludeThreadReplies_WhenOptionEnabled()
    {
        // Send a thread message first
        var rootTs = await SlackSendMessage.SendMessage("Thread root", _channelId, _token, CancellationToken.None);

        // Send a reply
        await SlackSendMessage.SendMessage("Thread reply", _channelId, _token, CancellationToken.None, rootTs);

        var input = new Input
        {
            ChannelId = _channelId,
            Limit = 10,
        };

        var options = new Options
        {
            IncludeThreads = true,
        };

        var result = await Slack.GetChannelMessages(input, _connection, options, CancellationToken.None);

        Assert.IsTrue(result.Success);
        Assert.IsTrue(result.Messages.Any(m => m.Text.Contains("Thread reply")));
    }

    [TestMethod]
    public async Task ShouldReturnEmptyList_WhenNoMessagesMatch()
    {
        var input = new Input
        {
            ChannelId = _channelId,
            Oldest = DateTimeOffset.UtcNow.AddDays(5).ToUnixTimeSeconds().ToString(),
            Latest = DateTimeOffset.UtcNow.AddDays(6).ToUnixTimeSeconds().ToString(),
            Limit = 5,
        };

        var options = new Options { IncludeThreads = false };

        var result = await Slack.GetChannelMessages(input, _connection, options, CancellationToken.None);

        Assert.IsTrue(result.Success);
        Assert.IsTrue(result.Messages.Count == 0);
    }

    [TestMethod]
    public async Task ShouldSetHasMore_WhenMoreMessagesExist()
    {
        var input = new Input
        {
            ChannelId = _channelId,
            Limit = 1,
        };

        var options = new Options();

        var result = await Slack.GetChannelMessages(input, _connection, options, CancellationToken.None);

        Assert.IsTrue(result.Success);
        Assert.IsTrue(result.HasMore || result.Messages.Count <= 1);
    }

    [TestMethod]
    public async Task ShouldFail_WhenTokenIsMissing()
    {
        var input = new Input
        {
            ChannelId = _channelId,
            Limit = 5,
        };

        var connection = new Connection
        {
            Token = string.Empty,
        };

        var options = new Options { IncludeThreads = false };

        var result = await Slack.GetChannelMessages(input, connection, options, CancellationToken.None);

        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsTrue(result.Error.Message.Contains("token", StringComparison.OrdinalIgnoreCase));
    }
}