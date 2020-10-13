using FacebookCore.APIs;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace FacebookCore.IntegrationTests
{
    public class FacebookCoreTest
    {
        private readonly FacebookAppApi _facebookAppApi;

        public FacebookCoreTest()
        {
            var basePath = Directory.GetCurrentDirectory();

            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

            var configurationRoot = builder.Build();

            var clientId = configurationRoot["client_id"];
            var clientSecret = configurationRoot["client_secret"];

            _facebookAppApi = new FacebookAppApi(new FacebookClient(clientId, clientSecret));
        }

        [Fact]
        public async Task ShouldGetApplicationId()
        {
            var appId = await _facebookAppApi.GetAppIdAsync();
            appId.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ShouldBeAbleToGetAccessToken()
        {
            var token = await _facebookAppApi.GetAccessTokenAsync();
            token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ShouldBeAbleToGetTestUsersList()
        {
            var users = await _facebookAppApi.GetTestUsersAsync();
            users.Should().NotBeNull();
            users.Count.Should().BeGreaterThan(0);
        }

    }
}
