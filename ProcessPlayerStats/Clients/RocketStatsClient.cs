using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProcessPlayerStats.Clients
{
    public class RocketStatsClient : IRocketStatProviderClient
    {
        private readonly IHttpClient httpClient;

        private readonly string RocketStatsUrl;

        public RocketStatsClient(IConfiguration configuration, IHttpClient httpClient)
        {
            this.httpClient = httpClient;

            RocketStatsUrl = configuration["RocketStats:Url"];
        }

        public async Task GetRecentRocketLeagueSessionsAsync(string userId, string platform)
        {
            var request = new RestRequest(Method.GET);
            var url = $"{RocketStatsUrl}/api/v2/rocket-league/standard/profile/{platform}/{userId}/sessions?";

            var response = await httpClient.ExecuteRequestAsync<string>(url, request);
        } 
    }
}
