using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using ProcessPlayerStats.Clients;
using ProcessPlayerStats.Dtos.Response;
using ProcessPlayerStats.Dtos.RTDtos;
using RestSharp;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ProcessPlayerStats.UnitTests
{
    public class HttpHelperClientUnitTests
    {
        private OAuthResponseDto testingDto => new OAuthResponseDto()
        {
            access_token = "Fake Token Ya Know",
            expires_in = 3600,
            token_type = "bearer",
            scope = "The.Universe"
        };
        private const string testingUrl = "https://noneofyoubeeswax.com/cool-stuff";

        private Mock<IRestClient> mockRestClient;
        private Mock<ILogger> mockLogger;
        private HttpHelperClient httpClient;

        public HttpHelperClientUnitTests()
        {
            mockRestClient = new Mock<IRestClient>();
            mockLogger = new Mock<ILogger>();

            httpClient = new HttpHelperClient(mockRestClient.Object, mockLogger.Object);
        }

        [Theory]
        [InlineData(HttpStatusCode.OK)]
        [InlineData(HttpStatusCode.Created)]
        [InlineData(HttpStatusCode.NoContent)]
        public async Task ExecuteRequestAsync_Empty_HappyPaths(HttpStatusCode statusCode)
        {
            // Arrange
            var request = new RestRequest() { };
            var restResponse = new RestResponse<OAuthResponseDto>() { StatusCode = statusCode };
            mockRestClient.Setup(client => client.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(restResponse);

            // Act
            await httpClient.ExecuteRequestAsync(testingUrl, request);

            // Assert
            mockRestClient.Verify(client => client.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.Unauthorized)]
        [InlineData(HttpStatusCode.Forbidden)]
        [InlineData(HttpStatusCode.InternalServerError)]
        public async Task ExecuteRequestAsync_BadResponseEmpty_SadPaths(HttpStatusCode statusCode)
        {
            // Arrange
            var request = new RestRequest() { };
            var restResponse = new RestResponse<OAuthResponseDto>() { StatusCode = statusCode };
            mockRestClient.Setup(client => client.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(restResponse);

            // Act
            await Assert.ThrowsAsync<HttpRequestException>( async () => await httpClient.ExecuteRequestAsync(testingUrl, request));

            // Assert
            mockRestClient.Verify(client => client.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(HttpHelperClient.RetryCount + 1));
        }

        [Theory]
        [InlineData(HttpStatusCode.OK)]
        [InlineData(HttpStatusCode.Created)]
        [InlineData(HttpStatusCode.NoContent)]
        public async Task ExecuteRequestAsync_ObjectResponse_HappyPaths(HttpStatusCode statusCode)
        {
            // Arrange
            var request = new RestRequest() { };
            var serializedBody = JsonConvert.SerializeObject(testingDto);
            var restResponse = new RestResponse<OAuthResponseDto>() { Content = serializedBody, StatusCode = statusCode };
            mockRestClient.Setup(client => client.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(restResponse);

            // Act
            var response = await httpClient.ExecuteRequestAsync<OAuthResponseDto>(testingUrl, request);

            // Assert
            Assert.Equal(testingDto.access_token, response.access_token);
            mockRestClient.Verify(client => client.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.Unauthorized)]
        [InlineData(HttpStatusCode.Forbidden)]
        [InlineData(HttpStatusCode.InternalServerError)]
        public async Task ExecuteRequestAsync_ObjectBadResponse_SadPaths(HttpStatusCode statusCode)
        {
            // Arrange
            var request = new RestRequest() { };
            var serializedBody = JsonConvert.SerializeObject(testingDto);
            var restResponse = new RestResponse<OAuthResponseDto>() { Content = serializedBody, StatusCode = statusCode };
            mockRestClient.Setup(client => client.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(restResponse);

            // Act
            await Assert.ThrowsAsync<HttpRequestException>(async () => await httpClient.ExecuteRequestAsync<OAuthResponseDto>(testingUrl, request));

            // Assert
            mockRestClient.Verify(client => client.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(HttpHelperClient.RetryCount + 1));
        }

        //[Fact]
        //public async Task ExecuteRequestAsync_FailedToDeserializeResponse_SadPath()
        //{
        //    // Arrange
        //    var statusCode = HttpStatusCode.OK;
        //    var request = new RestRequest() { };
        //    var serializedBody = JsonConvert.SerializeObject(testingDto);
        //    var restResponse = new RestResponse<OAuthResponseDto>() { Content = serializedBody, StatusCode = statusCode };
        //    mockRestClient.Setup(client => client.ExecuteAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(restResponse);

        //    // Act
        //    await Assert.ThrowsAsync<Exception>(async () => await httpClient.ExecuteRequestAsync<RTData>(testingUrl, request));
        //}
    }
}
