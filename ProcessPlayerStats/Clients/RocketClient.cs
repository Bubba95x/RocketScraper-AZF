using API.RocketStats.Dtos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ProcessPlayerStats.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProcessPlayerStats.Clients
{
    public class RocketClient : IRocketClient
    {
        private readonly IAuthClient authClient;
        private readonly IHttpClient httpClient;

        private readonly string rocketApiUrl;

        public RocketClient(IAuthClient authClient, IConfiguration configuration, IHttpClient httpClient)
        {
            this.authClient = authClient;
            this.httpClient = httpClient;

            rocketApiUrl = configuration["RocketApi:Url"];
        }

        public async Task<List<PlayerDto>> GetAllPlayersAsync()
        {
            var token = await authClient.ObtainAccessTokenAsync();
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {token}");

            return await httpClient.ExecuteRequestAsync<List<PlayerDto>>($"{rocketApiUrl}/api/user/list", request);
        }

        public async Task PostRocketStatsMatchAsync(Guid userId, RTMatchRequestDto requestDto)
        {
            var token = await authClient.ObtainAccessTokenAsync();
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddJsonBody(JsonConvert.SerializeObject(requestDto));

            await httpClient.ExecuteRequestAsync($"{rocketApiUrl}/api/RocketTracker/match/user/{userId}", request);
        }
    }
}
