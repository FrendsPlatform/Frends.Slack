using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Frends.Slack.ListChannels.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.Slack.ListChannels.Tests;

[TestClass]
public class UnitTests
{
    private readonly string _token = Environment.GetEnvironmentVariable("FRENDS_SLACK_TEST_TOKEN");
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
    public async Task ShouldFetchPublicChannels_Basic()
    {
        var options = new Options
        {
            ExcludeArchived = true,
            Limit = 3,
        };

        var result = await Slack.ListChannels(_connection, options, CancellationToken.None);

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Channels);
        Assert.IsTrue(result.Channels.Count > 0);
        Assert.IsTrue(result.Channels.All(c => !string.IsNullOrEmpty(c.Id) && !string.IsNullOrEmpty(c.Name)));
    }

    [TestMethod]
    public async Task ShouldNotIncludeArchivedChannels_WhenExcluded()
    {
        var options = new Options
        {
            ExcludeArchived = true,
            Limit = 3,
        };

        var result = await Slack.ListChannels(_connection, options, CancellationToken.None);

        Assert.IsTrue(result.Success);
        Assert.IsFalse(result.Channels.Any(c => c.IsArchived));
    }

    [TestMethod]
    public async Task ShouldIncludeArchivedChannels_WhenOptionIsFalse()
    {
        var options = new Options
        {
            ExcludeArchived = false,
            Limit = 6,
        };

        var result = await Slack.ListChannels(_connection, options, CancellationToken.None);

        Assert.IsTrue(result.Success);
        Assert.IsTrue(result.Channels.Any(c => c.IsArchived));
    }

    [TestMethod]
    public async Task ShouldListLimitedChannels_AndSetHasMoreCorrectly()
    {
        var options = new Options
        {
            Limit = 2,
            ExcludeArchived = true,
        };

        var result = await Slack.ListChannels(_connection, options, CancellationToken.None);

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Channels);
        Assert.IsTrue(result.Channels.Count <= 2);

        // If total channels > 2, HasMore should be true
        if (result.Channels.Count == 2)
        {
            Assert.IsTrue(result.HasMore);
            Assert.IsFalse(string.IsNullOrEmpty(result.NextCursor));
        }
        else
        {
            // Less channels than limit, so no more pages
            Assert.IsFalse(result.HasMore);
            Assert.IsNull(result.NextCursor);
        }
    }

    [TestMethod]
    public async Task ShouldFetchNextPage_WhenCursorProvided()
    {
        // First, get first 2 channels with HasMore and cursor
        var optionsFirstPage = new Options
        {
            Limit = 2,
            ExcludeArchived = true,
            Cursor = null,
        };

        var firstPageResult = await Slack.ListChannels(_connection, optionsFirstPage, CancellationToken.None);

        Assert.IsTrue(firstPageResult.Success);
        Assert.IsTrue(firstPageResult.Channels.Count <= 2);

        if (!firstPageResult.HasMore)
        {
            Assert.IsNull(firstPageResult.NextCursor);
            return; // no next page to test
        }

        // Then fetch next page using NextCursor
        var optionsSecondPage = new Options
        {
            Limit = 2,
            ExcludeArchived = true,
            Cursor = firstPageResult.NextCursor,
        };

        var secondPageResult = await Slack.ListChannels(_connection, optionsSecondPage, CancellationToken.None);

        Assert.IsTrue(secondPageResult.Success);
        Assert.IsTrue(secondPageResult.Channels.Count > 0);

        // Combined channels from both pages should be unique
        var allChannelIds = firstPageResult.Channels.Select(c => c.Id)
                            .Concat(secondPageResult.Channels.Select(c => c.Id))
                            .Distinct();

        Assert.AreEqual(firstPageResult.Channels.Count + secondPageResult.Channels.Count, allChannelIds.Count());
    }

    [TestMethod]
    public async Task ShouldFailWithInvalidToken()
    {
        var badConnection = new Connection { Token = "invalid-token" };
        var options = new Options
        {
            ThrowErrorOnFailure = false,
            Limit = 10,
        };

        var result = await Slack.ListChannels(badConnection, options, CancellationToken.None);

        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsTrue(result.Error.Message.Contains("invalid_auth") || result.Error.Message.Contains("Slack API error"));
    }

    [TestMethod]
    public async Task ShouldThrowException_WhenThrowErrorOptionIsTrue()
    {
        var badConnection = new Connection { Token = "invalid-token" };
        var options = new Options
        {
            ThrowErrorOnFailure = true,
            Limit = 10,
        };

        await Assert.ThrowsExactlyAsync<Exception>(async () =>
        {
            await Slack.ListChannels(badConnection, options, CancellationToken.None);
        });
    }
}
