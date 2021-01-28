using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProcessPlayerStats.Dtos.Request;
using ProcessPlayerStats.Dtos.Response;
using System;
using System.Threading.Tasks;

namespace ProcessPlayerStats.Clients
{
    public class RocketClientDecorator : RocketClient, IRocketClient
    {
        private readonly IDistributedCache cache;
        public DistributedCacheEntryOptions DefaultCachingOptions => new DistributedCacheEntryOptions() { SlidingExpiration = TimeSpan.FromDays(2) };

        public RocketClientDecorator(IAuthClient authClient, IConfiguration configuration, ILogger logger, IHttpHelperClient http, IDistributedCache cache) 
            : base(authClient, configuration, logger, http)
        {
            this.cache = cache;
        }

        public static string PlayerMatchRocketStatsKey(Guid rocketStatsID)
        {
            return $"PlayerMatch:{rocketStatsID}";
        }

        public async override Task<PlayerMatchResponseDto> GetPlayerMatchByRocketIdAsync(Guid rocketStatsID)
        {
            string key = PlayerMatchRocketStatsKey(rocketStatsID);
            var playerMatchSerialized = await cache.GetStringAsync(key);
            
            if (!string.IsNullOrEmpty(playerMatchSerialized))
            {
                try
                {
                    return JsonConvert.DeserializeObject<PlayerMatchResponseDto>(playerMatchSerialized);
                }
                catch
                {

                }            
            }

            var response = await base.GetPlayerMatchByRocketIdAsync(rocketStatsID);
            await cache.SetStringAsync(key, JsonConvert.SerializeObject(response), DefaultCachingOptions);
            return response;                     
        }

        public async override Task<PlayerMatchResponseDto> AddPlayerMatchAsync(PlayerMatchRequestDto matchStatistic)
        {
            var response = await base.AddPlayerMatchAsync(matchStatistic);
            var key = PlayerMatchRocketStatsKey(response.RocketStatsID);
            await cache.SetStringAsync(key, JsonConvert.SerializeObject(response), DefaultCachingOptions);
            return response;
        }
    }
}
