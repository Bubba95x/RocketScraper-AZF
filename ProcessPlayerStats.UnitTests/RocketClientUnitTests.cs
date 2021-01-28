using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using ProcessPlayerStats.Clients;
using ProcessPlayerStats.Dtos.Request;
using ProcessPlayerStats.Dtos.Response;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ProcessPlayerStats.UnitTests
{
    public class RocketClientUnitTests
    {
        private const string fakeUrl = "https://www.somewebsite.com";
        private const string fakeToken = "Bear IamSuperFakeToken";

        private readonly Mock<IAuthClient> mockAuthClient;
        private readonly Mock<IConfiguration> mockConfiguration;
        private readonly Mock<IHttpHelperClient> mockHttpClient;
        private readonly Mock<ILogger> mockLogger;

        private readonly RocketClient rocketClient;
        
        public RocketClientUnitTests()
        {
            mockAuthClient = new Mock<IAuthClient>();
            mockConfiguration = new Mock<IConfiguration>();
            mockHttpClient = new Mock<IHttpHelperClient>();
            mockLogger = new Mock<ILogger>();

            mockConfiguration.Setup(x => x[It.Is<string>(s => s == "RocketApi:Url")]).Returns(fakeUrl);
            mockAuthClient.Setup(client => client.ObtainAccessTokenAsync()).ReturnsAsync(fakeToken);

            rocketClient = new RocketClient(mockAuthClient.Object, mockConfiguration.Object, mockLogger.Object, mockHttpClient.Object);
        }

        [Fact]
        public async Task GetAllPlayersAsync_HappyPath()
        {
            // Arrange
            var player1 = new PlayerResponseDto()
            {
                Id = Guid.NewGuid(),
                UserName = "Starlord",
                PlatformName = "Steam",
                AvatarUrl = "https://www.pictures.com",
                RocketStatsID = "Starlord",
                DateCreatedUTC = DateTime.UtcNow,
                DateModifiedUTC = DateTime.UtcNow
            };
            var playerList = new List<PlayerResponseDto>();
            playerList.Add(player1);            
            mockHttpClient.Setup(client => client.ExecuteRequestAsync<List<PlayerResponseDto>>($"{fakeUrl}/api/player/list", It.IsAny<RestRequest>())).ReturnsAsync(playerList);

            // Act
            var response = await rocketClient.GetAllPlayersAsync();

            // Assert
            mockHttpClient.Verify(Clients => Clients.ExecuteRequestAsync<List<PlayerResponseDto>>($"{fakeUrl}/api/player/list", It.IsAny<RestRequest>()), Times.Once);
            Assert.NotNull(response);
            Assert.Equal(playerList.Count, response.Count);
        }

        [Fact]
        public async Task AddMatchAsync_HappyPath()
        {
            // Arrange
            var match = new MatchResponseDto()
            {
                ID = Guid.NewGuid(),
                GameMode = "Doubles",
                MatchDate = DateTime.UtcNow,
                DateCreatedUTC = DateTime.UtcNow,
                DateModifiedUTC = DateTime.UtcNow
            };
            var matchRequest = new MatchRequestDto()
            {
                MatchDate = match.MatchDate,
                GameMode = match.GameMode
            };
            mockHttpClient.Setup(client => client.ExecuteRequestAsync<MatchResponseDto>($"{fakeUrl}/api/match", It.IsAny<RestRequest>())).ReturnsAsync(match);

            // Act
            var response = await rocketClient.AddMatchAsync(matchRequest);

            // Assert
            mockHttpClient.Verify(Clients => Clients.ExecuteRequestAsync<MatchResponseDto>($"{fakeUrl}/api/match", It.IsAny<RestRequest>()), Times.Once);
            Assert.NotNull(response);
            Assert.Equal(matchRequest.GameMode, response.GameMode);
            Assert.Equal(matchRequest.MatchDate, response.MatchDate);
            Assert.Equal(match.ID, response.ID);
        }

        [Fact]
        public async Task GetMatchAsync_HappyPath()
        {
            // Arrange
            Guid fakeId = Guid.NewGuid();
            var match = new MatchResponseDto()
            {
                ID = fakeId,
                GameMode = "Doubles",
                MatchDate = DateTime.UtcNow,
                DateCreatedUTC = DateTime.UtcNow,
                DateModifiedUTC = DateTime.UtcNow
            };
            mockHttpClient.Setup(client => client.ExecuteRequestAsync<MatchResponseDto>($"{fakeUrl}/api/match/{fakeId}", It.IsAny<RestRequest>())).ReturnsAsync(match);

            // Act
            var response = await rocketClient.GetMatchAsync(fakeId);

            // Assert
            mockHttpClient.Verify(Clients => Clients.ExecuteRequestAsync<MatchResponseDto>($"{fakeUrl}/api/match/{fakeId}", It.IsAny<RestRequest>()), Times.Once);
            Assert.NotNull(response);
            Assert.Equal(match.GameMode, response.GameMode);
            Assert.Equal(match.MatchDate, response.MatchDate);
            Assert.Equal(match.ID, response.ID);
        }

        [Fact]
        public async Task AddPlayerMatchStatisticAsync_HappyPath()
        {
            // Arrange
            var fakeStat = new PlayerMatchStatisticRequestDto() { PlayerMatchId = Guid.NewGuid(), StatType = "Goals", Value = 8 };

            // Act
            await rocketClient.AddPlayerMatchStatisticAsync(fakeStat);

            // Assert
            mockHttpClient.Verify(Clients => Clients.ExecuteRequestAsync($"{fakeUrl}/api/playermatchstatistic", It.IsAny<RestRequest>()), Times.Once);
        }

        [Fact]
        public async Task AddPlayerMatchAsync_NullMatch_HappyPath()
        {
            // Arrange
            Guid? fakeMatchId = null;
            var fakePlayerMatchRequest = new PlayerMatchRequestDto() {
                PlayerID = Guid.NewGuid(),
                MatchID = fakeMatchId,
                Victory = "Victory",
                RocketStatsID = Guid.NewGuid(),
                RocketStatsGameMode = "Doubles",
                RocketStatsMatchDate = DateTime.UtcNow
            };

            var fakePlayerMatchResponse = new PlayerMatchResponseDto()
            {
                ID = Guid.NewGuid(),
                PlayerID = fakePlayerMatchRequest.PlayerID,
                MatchID = fakePlayerMatchRequest.MatchID,
                Victory = fakePlayerMatchRequest.Victory,
                RocketStatsID = fakePlayerMatchRequest.RocketStatsID,
                RocketStatsGameMode = fakePlayerMatchRequest.RocketStatsGameMode,
                RocketStatsMatchDate = fakePlayerMatchRequest.RocketStatsMatchDate
            };
            mockHttpClient.Setup(client => client.ExecuteRequestAsync<PlayerMatchResponseDto>(
                $"{fakeUrl}/api/playermatch", It.IsAny<RestRequest>())).ReturnsAsync(fakePlayerMatchResponse);

            // Act
            var response = await rocketClient.AddPlayerMatchAsync(fakePlayerMatchRequest);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(fakePlayerMatchResponse.ID, response.ID);
            Assert.Equal(fakePlayerMatchRequest.PlayerID, response.PlayerID);
            Assert.Equal(fakePlayerMatchRequest.MatchID, fakeMatchId);
            Assert.Equal(fakePlayerMatchRequest.RocketStatsID, response.RocketStatsID);
            Assert.Equal(fakePlayerMatchRequest.RocketStatsGameMode, response.RocketStatsGameMode);
            Assert.Equal(fakePlayerMatchRequest.RocketStatsMatchDate, response.RocketStatsMatchDate);
        }

        [Fact]
        public async Task AddPlayerMatchAsync_HappyPath()
        {
            // Arrange
            Guid? fakeMatchId = Guid.NewGuid();
            var fakePlayerMatchRequest = new PlayerMatchRequestDto()
            {
                PlayerID = Guid.NewGuid(),
                MatchID = fakeMatchId,
                Victory = "Victory",
                RocketStatsID = Guid.NewGuid(),
                RocketStatsGameMode = "Doubles",
                RocketStatsMatchDate = DateTime.UtcNow
            };

            var fakePlayerMatchResponse = new PlayerMatchResponseDto()
            {
                ID = Guid.NewGuid(),
                PlayerID = fakePlayerMatchRequest.PlayerID,
                MatchID = fakePlayerMatchRequest.MatchID,
                Victory = fakePlayerMatchRequest.Victory,
                RocketStatsID = fakePlayerMatchRequest.RocketStatsID,
                RocketStatsGameMode = fakePlayerMatchRequest.RocketStatsGameMode,
                RocketStatsMatchDate = fakePlayerMatchRequest.RocketStatsMatchDate
            };
            mockHttpClient.Setup(client => client.ExecuteRequestAsync<PlayerMatchResponseDto>(
                $"{fakeUrl}/api/playermatch", It.IsAny<RestRequest>())).ReturnsAsync(fakePlayerMatchResponse);

            // Act
            var response = await rocketClient.AddPlayerMatchAsync(fakePlayerMatchRequest);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(fakePlayerMatchResponse.ID, response.ID);
            Assert.Equal(fakePlayerMatchRequest.PlayerID, response.PlayerID);
            Assert.Equal(fakePlayerMatchRequest.MatchID, fakeMatchId);
            Assert.Equal(fakePlayerMatchRequest.RocketStatsID, response.RocketStatsID);
            Assert.Equal(fakePlayerMatchRequest.RocketStatsGameMode, response.RocketStatsGameMode);
            Assert.Equal(fakePlayerMatchRequest.RocketStatsMatchDate, response.RocketStatsMatchDate);
        }

        [Fact]
        public async Task GetPlayerMatchAsync_HappyPath()
        {
            // Arrange
            Guid playerId = Guid.NewGuid();
            Guid matchId = Guid.NewGuid();
            var fakePlayerMatchRequest = new PlayerMatchRequestDto()
            {
                PlayerID = playerId,
                MatchID = matchId,
                Victory = "Victory",
                RocketStatsID = Guid.NewGuid(),
                RocketStatsGameMode = "Doubles",
                RocketStatsMatchDate = DateTime.UtcNow
            };

            var fakePlayerMatchResponse = new PlayerMatchResponseDto()
            {
                ID = Guid.NewGuid(),
                PlayerID = fakePlayerMatchRequest.PlayerID,
                MatchID = fakePlayerMatchRequest.MatchID,
                Victory = fakePlayerMatchRequest.Victory,
                RocketStatsID = fakePlayerMatchRequest.RocketStatsID,
                RocketStatsGameMode = fakePlayerMatchRequest.RocketStatsGameMode,
                RocketStatsMatchDate = fakePlayerMatchRequest.RocketStatsMatchDate
            };
            mockHttpClient.Setup(client => client.ExecuteRequestAsync<PlayerMatchResponseDto>(
                $"{fakeUrl}/api/playermatch/player/{playerId}/match/{matchId}", It.IsAny<RestRequest>())).ReturnsAsync(fakePlayerMatchResponse);

            // Act
            var response = await rocketClient.GetPlayerMatchAsync(playerId, matchId);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(fakePlayerMatchResponse.ID, response.ID);
            Assert.Equal(fakePlayerMatchRequest.PlayerID, response.PlayerID);
            Assert.Equal(fakePlayerMatchRequest.MatchID, response.MatchID);
            Assert.Equal(fakePlayerMatchRequest.RocketStatsID, response.RocketStatsID);
            Assert.Equal(fakePlayerMatchRequest.RocketStatsGameMode, response.RocketStatsGameMode);
            Assert.Equal(fakePlayerMatchRequest.RocketStatsMatchDate, response.RocketStatsMatchDate);
        }

        [Fact]
        public async Task GetPlayerMatchByRocketIdAsync_HappyPath()
        {
            // Arrange
            Guid fakeId = Guid.NewGuid();
            var fakePlayerMatchResponse = new PlayerMatchResponseDto()
            {
                ID = Guid.NewGuid(),
                PlayerID = Guid.NewGuid(),
                MatchID = Guid.NewGuid(),
                Victory = "Fake Victory",
                RocketStatsID = fakeId,
                RocketStatsGameMode = "Victory 443",
                RocketStatsMatchDate = DateTime.UtcNow
            };
            mockHttpClient.Setup(client => client.ExecuteRequestAsync<PlayerMatchResponseDto>(
                $"{fakeUrl}/api/playermatch/rocketstatsid/{fakeId}", It.IsAny<RestRequest>())).ReturnsAsync(fakePlayerMatchResponse);

            // Act
            var response = await rocketClient.GetPlayerMatchByRocketIdAsync(fakeId);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(fakePlayerMatchResponse.ID, response.ID);
            Assert.Equal(fakePlayerMatchResponse.PlayerID, response.PlayerID);
            Assert.Equal(fakePlayerMatchResponse.MatchID, response.MatchID);
            Assert.Equal(fakePlayerMatchResponse.RocketStatsID, response.RocketStatsID);
            Assert.Equal(fakePlayerMatchResponse.RocketStatsGameMode, response.RocketStatsGameMode);
            Assert.Equal(fakePlayerMatchResponse.RocketStatsMatchDate, response.RocketStatsMatchDate);
        }
    }
}
