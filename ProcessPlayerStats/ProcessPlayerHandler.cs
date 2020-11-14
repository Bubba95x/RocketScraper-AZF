using Microsoft.Extensions.Logging;
using ProcessPlayerStats.Clients;
using ProcessPlayerStats.Models;
using System;
using System.Threading.Tasks;

namespace ProcessPlayerStats
{
    public class ProcessPlayerHandler : IHandler
    {
        private readonly ILogger logger;
        private readonly IRocketStatProviderClient statProvider;
        private readonly IRocketClient rocketClient;

        public ProcessPlayerHandler(ILoggerFactory loggerFactory, IRocketStatProviderClient statProvider, IRocketClient rocketClient)
        {
            this.statProvider = statProvider;
            this.rocketClient = rocketClient;
        }

        public async Task<string> ProcessEventAsync(PlayerDto player)
        {
            var session = await statProvider.GetRecentRocketLeagueSessionsAsync(player.RocketStatsID, player.PlatformName);
            foreach(var item in session.Data.Items)
            {
                foreach(var match in item.Matches)
                {
                    if(match.Metadata.Result == "victory" || match.Metadata.Result == "defeat")
                    {
                        try
                        {
                            await rocketClient.PostRocketStatsMatchAsync(player.Id, match);
                        }
                        catch(Exception ex)
                        {
                            logger.LogError(ex, $"Failed to post match ({match.ID}) for user ({player.UserName}|{player.Id})");
                        }                        
                    }
                }
            }
            return "Success";
        }
    }
}
