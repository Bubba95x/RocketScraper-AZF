using Microsoft.Extensions.Configuration;
using ProcessPlayerStats.Dtos.RTDtos;
using RestSharp;
using System.Threading.Tasks;

namespace ProcessPlayerStats.Clients
{
    public class RocketStatsClient : IRocketStatsClient
    {
        private readonly IHttpHelperClient httpClient;

        private readonly string RocketStatsUrl;

        public RocketStatsClient(IConfiguration configuration, IHttpHelperClient httpClient)
        {
            this.httpClient = httpClient;

            RocketStatsUrl = configuration["RocketStats:Url"];
        }

        public async Task<RTSession> GetRecentRocketLeagueSessionsAsync(string userId, string platform)
        {
            var request = new RestRequest(Method.GET);
            var url = $"{RocketStatsUrl}/api/v2/rocket-league/standard/profile/{platform.ToLower()}/{userId}/sessions?";

            return await httpClient.ExecuteRequestAsync<RTSession>(url, request);
        } 
    }
}
