using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using ProcessPlayerStats.Clients;
using ProcessPlayerStats.Dtos;
using ProcessPlayerStats.Dtos.Response;
using RestSharp;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ProcessPlayerStats.UnitTests
{
    public class IdentityServerAuthClientUnitTests
    {
        public readonly Mock<IConfiguration> mockConfiguration;
        public readonly Mock<ILogger> mockLogger;
        public readonly Mock<IHttpHelperClient> mockHttpClient;

        public readonly IdentityServerAuthClient authClient;

        private const string fakeClientId = "FakeClientId";
        private const string fakeSecretId = "FakeClientSecret";
        private const string fakeTokenUrl = "https://helloworld.com";
        private const string fakeToken = "Fake Token";
        private const int fakeExpiresIn = 3600;

        public IdentityServerAuthClientUnitTests()
        {
            mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(config => config["client_id"]).Returns(fakeClientId);
            mockConfiguration.Setup(config => config["client_secret"]).Returns(fakeSecretId);
            mockConfiguration.Setup(config => config["OAuth:Domain"]).Returns(fakeTokenUrl);
            mockLogger = new Mock<ILogger>();
            mockHttpClient = new Mock<IHttpHelperClient>();

            authClient = new IdentityServerAuthClient(mockConfiguration.Object, mockHttpClient.Object, mockLogger.Object);
        }

        [Fact]
        public async Task ObtainAccessTokenAsync_HappyPath_GetToken()
        {
            // Arrange
            var authTokenDto = new OAuthResponseDto() { access_token = fakeToken, expires_in = fakeExpiresIn };
            mockHttpClient.Setup(client => client.ExecuteRequestAsync<OAuthResponseDto>(It.IsAny<string>(), It.IsAny<IRestRequest>())).ReturnsAsync(authTokenDto);

            // Act
            var response = await authClient.ObtainAccessTokenAsync();

            // Assert
            Assert.Equal(fakeToken, response);
            mockHttpClient.Verify(client => client.ExecuteRequestAsync<OAuthResponseDto>(It.IsAny<string>(), It.IsAny<IRestRequest>()), Times.Once);
        }

        [Fact]
        public async Task ObtainAccessTokenAsync_HappyPath_GetTokenTwice()
        {
            // Arrange
            var authTokenDto = new OAuthResponseDto() { access_token = fakeToken, expires_in = fakeExpiresIn };
            var restResponse = new RestResponse<OAuthResponseDto>();
            restResponse.Content = JsonConvert.SerializeObject(authTokenDto);
            mockHttpClient.Setup(client => client.ExecuteRequestAsync<OAuthResponseDto>(It.IsAny<string>(), It.IsAny<IRestRequest>())).ReturnsAsync(authTokenDto);

            // Act
            var response1 = await authClient.ObtainAccessTokenAsync();
            var response2 = await authClient.ObtainAccessTokenAsync();

            // Assert
            Assert.Equal(fakeToken, response1);
            Assert.Equal(fakeToken, response2);
            mockHttpClient.Verify(client => client.ExecuteRequestAsync<OAuthResponseDto>(It.IsAny<string>(), It.IsAny<IRestRequest>()), Times.Once);
        }
    }
}
