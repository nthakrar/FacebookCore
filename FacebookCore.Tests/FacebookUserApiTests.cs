using FacebookCore.APIs;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace FacebookCore.IntegrationTests
{
    public class FacebookUserApiTests
    {
        private readonly FacebookUserApi _facebookUserApi;

        public FacebookUserApiTests()
        {
            string basePath = Directory.GetCurrentDirectory();

            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

            IConfigurationRoot configurationRoot = builder.Build();

            var clientId = configurationRoot["client_id"];
            var clientSecret = configurationRoot["client_secret"];

            var accessToken = configurationRoot["access_token"];
            _facebookUserApi = new FacebookUserApi(new FacebookClient(clientId, clientSecret), accessToken);
        }

        [Fact]
        public async Task ShouldExtendUserToken()
        {

            var extendResult = await _facebookUserApi.RequestExtendAccessToken();

            extendResult["access_token"].ToString().Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task ShouldGetUserInfo()
        {
       
            var extendResult = await _facebookUserApi.RequestInformationAsync();

            extendResult["id"].ToString().Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task ShouldGetUserMetaInfo()
        {
          
            var metadata = await _facebookUserApi.RequestMetaDataAsync();

            metadata["metadata"].ToString().Should().NotBeNullOrWhiteSpace();
        }
    }
}
