using Microsoft.Extensions.Configuration;
using ProcessPlayerStats.Models;
using RestSharp;
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
            var request = new RestRequest();
            request.AddHeader("Authorization", $"Bearer {token}");

            return await httpClient.ExecuteRequestAsync<List<PlayerDto>>($"{rocketApiUrl}/api/user/list", request);
        }

        public async Task PostRocketStatsMatchAsync()
        {

        }
    }
}
