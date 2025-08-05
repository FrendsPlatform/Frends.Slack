using System;
using System.Threading;
using System.Threading.Tasks;
using Frends.Slack.GetChannelMessages.Definitions;
using Frends.Slack.GetUserInfo.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.Slack.GetUserInfo.Tests;

[TestClass]
public class UnitTests
{
    [TestClass]
    public class GetUserInfoTests
    {
        private readonly string _token = Environment.GetEnvironmentVariable("FRENDS_SLACK_TEST_TOKEN");
        private readonly string _validUserId = Environment.GetEnvironmentVariable("FRENDS_SLACK_TEST_USER_ID");

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
        public async Task ShouldFetchUserInfo_ValidUserId()
        {
            var input = new Input
            {
                UserId = _validUserId,
            };

            var options = new Options
            {
                ThrowErrorOnFailure = false,
                ErrorMessageOnFailure = "Failed to get user info",
            };

            var result = await Slack.GetUserInfo(input, _connection, options, CancellationToken.None);

            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.User);
            Assert.AreEqual(_validUserId, result.User.Id);
            Assert.IsFalse(string.IsNullOrEmpty(result.User.Name));
        }

        [TestMethod]
        public async Task ShouldFailWithInvalidUserId()
        {
            var input = new Input
            {
                UserId = "INVALIDUSERID",
            };

            var options = new Options
            {
                ThrowErrorOnFailure = false,
            };

            var result = await Slack.GetUserInfo(input, _connection, options, CancellationToken.None);

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Error);
            Assert.IsTrue(result.Error.Message.Contains("user_not_found") || result.Error.Message.Contains("Slack API error"));
        }

        [TestMethod]
        public async Task ShouldFailWithMissingUserId()
        {
            var input = new Input
            {
                UserId = null,
            };

            var options = new Options
            {
                ThrowErrorOnFailure = false,
            };

            var result = await Slack.GetUserInfo(input, _connection, options, CancellationToken.None);

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Error);
            Assert.IsTrue(result.Error.Message.Contains("User ID is required"));
        }

        [TestMethod]
        public async Task ShouldFailWithMissingToken()
        {
            var input = new Input
            {
                UserId = _validUserId,
            };

            var badConnection = new Connection { Token = null };

            var options = new Options
            {
                ThrowErrorOnFailure = false,
            };

            var result = await Slack.GetUserInfo(input, badConnection, options, CancellationToken.None);

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Error);
            Assert.IsTrue(result.Error.Message.Contains("Slack token is required"));
        }
    }
}
