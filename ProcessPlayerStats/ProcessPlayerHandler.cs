using API.RocketStats.Dtos.RTDtos;
using Microsoft.Extensions.Logging;
using ProcessPlayerStats.Clients;
using ProcessPlayerStats.Dtos.Request;
using ProcessPlayerStats.Dtos.Response;
using System.Threading.Tasks;

namespace ProcessPlayerStats
{
    public class ProcessPlayerHandler : IHandler
    {
        private readonly ILogger logger;
        private readonly IRocketStatsClient statProvider;
        private readonly IRocketClient rocketClient;

        public ProcessPlayerHandler(ILogger logger, IRocketStatsClient statProvider, IRocketClient rocketClient)
        {
            this.statProvider = statProvider;
            this.rocketClient = rocketClient;

            this.logger = logger;
        }

        public async Task<string> ProcessEventAsync(PlayerResponseDto player)
        {
            var session = await statProvider.GetRecentRocketLeagueSessionsAsync(player.RocketStatsID, player.PlatformName);
            foreach(var item in session.Data.Items)
            {
                foreach(var rtMatch in item.Matches)
                {
                    if (rtMatch.Metadata.Result.ToLower() == "victory" || rtMatch.Metadata.Result.ToLower() == "defeat") // Indicates individual game
                    {
                        var existingPlayerMatch = await rocketClient.GetPlayerMatchByRocketIdAsync(rtMatch.ID);
                        
                        if(existingPlayerMatch == default)
                        {
                            var playermatch = await rocketClient.AddPlayerMatchAsync(new PlayerMatchRequestDto()
                            {
                                PlayerID = player.Id,
                                MatchID = null,
                                Victory = rtMatch.Metadata.Result.ToLower(),
                                RocketStatsID = rtMatch.ID,
                                RocketStatsGameMode = rtMatch.Metadata.Playlist,
                                RocketStatsMatchDate = rtMatch.Metadata.DateCollected
                            });

                            await ProcessMatchStatsAsync(playermatch, rtMatch.Stats);
                        }
                    }
                }
            }
            return "Success";
        }

        private async Task ProcessMatchStatsAsync(PlayerMatchResponseDto playerMatch, RTStatsDto stats)
        {
            // Assists
            await rocketClient.AddPlayerMatchStatisticAsync(new PlayerMatchStatisticRequestDto()
            {
                PlayerMatchId = playerMatch.ID,
                StatType = stats.Assists.DisplayName,
                Value = stats.Assists.Value ?? 0
            });

            // Goals
            await rocketClient.AddPlayerMatchStatisticAsync(new PlayerMatchStatisticRequestDto()
            {
                PlayerMatchId = playerMatch.ID,
                StatType = stats.Goals.DisplayName,
                Value = stats.Goals.Value ?? 0
            });

            // MVP
            await rocketClient.AddPlayerMatchStatisticAsync(new PlayerMatchStatisticRequestDto()
            {
                PlayerMatchId = playerMatch.ID,
                StatType = stats.Mvps.DisplayName,
                Value = stats.Mvps.Value ?? 0
            });

            // Saves
            await rocketClient.AddPlayerMatchStatisticAsync(new PlayerMatchStatisticRequestDto()
            {
                PlayerMatchId = playerMatch.ID,
                StatType = stats.Saves.DisplayName,
                Value = stats.Saves.Value ?? 0
            });

            // Shots
            await rocketClient.AddPlayerMatchStatisticAsync(new PlayerMatchStatisticRequestDto()
            {
                PlayerMatchId = playerMatch.ID,
                StatType = stats.Shots.DisplayName,
                Value = stats.Shots.Value ?? 0
            });
        }
    }
}
