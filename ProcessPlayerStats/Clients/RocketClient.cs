using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProcessPlayerStats.Dtos.Request;
using ProcessPlayerStats.Dtos.Response;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProcessPlayerStats.Clients
{
    public class RocketClient : IRocketClient
    {
        private readonly IAuthClient authClient;
        private readonly ILogger logger;
        private readonly IHttpHelperClient http;

        private readonly string rocketApiUrl;

        public RocketClient(IAuthClient authClient, IConfiguration configuration, ILogger logger, IHttpHelperClient http)
        {
            this.authClient = authClient;
            this.logger = logger;
            this.http = http;

            rocketApiUrl = configuration["RocketApi:Url"];
        }

        public virtual async Task<List<PlayerResponseDto>> GetAllPlayersAsync()
        {
            var url = $"{rocketApiUrl}/api/player/list";
            var token = await authClient.ObtainAccessTokenAsync();
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {token}");
             
            return await http.ExecuteRequestAsync<List<PlayerResponseDto>>(url, request);
        }
        
        public virtual async Task<MatchResponseDto> AddMatchAsync(MatchRequestDto match)
        {
            var url = $"{rocketApiUrl}/api/match";
            var token = await authClient.ObtainAccessTokenAsync();
            var request = new RestRequest(Method.POST);
            request.AddJsonBody(JsonConvert.SerializeObject(match));
            request.AddHeader("Authorization", $"Bearer {token}");

            return await http.ExecuteRequestAsync<MatchResponseDto>(url, request);
        }

        public virtual async Task<MatchResponseDto> GetMatchAsync(Guid ID)
        {
            var url = $"{rocketApiUrl}/api/match/{ID}";
            var token = await authClient.ObtainAccessTokenAsync();
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {token}");

            return await http.ExecuteRequestAsync<MatchResponseDto>(url, request);
        }

        public virtual async Task AddPlayerMatchStatisticAsync(PlayerMatchStatisticRequestDto matchStatistic)
        {
            var url = $"{rocketApiUrl}/api/playermatchstatistic";
            var token = await authClient.ObtainAccessTokenAsync();
            var request = new RestRequest(Method.POST);
            request.AddJsonBody(JsonConvert.SerializeObject(matchStatistic));
            request.AddHeader("Authorization", $"Bearer {token}");

            await http.ExecuteRequestAsync(url, request);
        }

        public virtual async Task<PlayerMatchResponseDto> AddPlayerMatchAsync(PlayerMatchRequestDto matchStatistic)
        {
            var url = $"{rocketApiUrl}/api/playermatch";
            var token = await authClient.ObtainAccessTokenAsync();
            var request = new RestRequest(Method.POST);
            request.AddJsonBody(JsonConvert.SerializeObject(matchStatistic));
            request.AddHeader("Authorization", $"Bearer {token}");

            return await http.ExecuteRequestAsync<PlayerMatchResponseDto>(url, request);
        }

        public virtual async Task<PlayerMatchResponseDto> GetPlayerMatchAsync(Guid playerId, Guid matchId)
        {
            var url = $"{rocketApiUrl}/api/playermatch/player/{playerId}/match/{matchId}";
            var token = await authClient.ObtainAccessTokenAsync();
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {token}");

            return await http.ExecuteRequestAsync<PlayerMatchResponseDto>(url, request);
        }

        public virtual async Task<PlayerMatchResponseDto> GetPlayerMatchByRocketIdAsync(Guid rocketStatsID)
        {
            var url = $"{rocketApiUrl}/api/playermatch/rocketstatsid/{rocketStatsID}";
            var token = await authClient.ObtainAccessTokenAsync();
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {token}");

            return await http.ExecuteRequestAsync<PlayerMatchResponseDto>(url, request);
        }
    }
}
