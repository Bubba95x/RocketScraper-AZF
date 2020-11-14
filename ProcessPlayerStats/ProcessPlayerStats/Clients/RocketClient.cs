using Microsoft.Extensions.Configuration;
using ProcessPlayerStats.Models;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProcessPlayerStats.Clients
{
    public class RocketClient : IRocketClient
    {
        private readonly IAuth0Client auth0Client;
        private readonly IHttpClient httpClient;

        private readonly string rocketApiUrl;

        public RocketClient(IAuth0Client auth0Client, IConfiguration configuration, IHttpClient httpClient)
        {
            this.auth0Client = auth0Client;
            this.httpClient = httpClient;

            rocketApiUrl = configuration["RocketApi:Url"];
        }

        public async Task<List<PlayerModel>> GetAllPlayersAsync()
        {
            var token = await auth0Client.ObtainAccessTokenAsync();
            var request = new RestRequest();
            request.AddHeader("Authorization", $"Bearer {token}");

            return await httpClient.ExecuteRequestAsync<List<PlayerModel>>($"{rocketApiUrl}/api/user/list", request);
        }
    }
}
