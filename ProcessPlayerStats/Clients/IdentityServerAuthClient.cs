using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProcessPlayerStats.Dtos.Response;
using RestSharp;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessPlayerStats.Clients
{
    public class IdentityServerAuthClient : IAuthClient
    {
        private string accessToken;
        private readonly string client_id;
        private readonly string client_secret;
        private readonly string oauthEndpoint;
        private const string grant_type = "client_credentials";
        private const string scope = "RocketAPI.Read RocketAPI.Write";
        private readonly Semaphore semaphore;
        private readonly IHttpHelperClient http;
        private readonly ILogger logger;

        public IdentityServerAuthClient(IConfiguration configuration, IHttpHelperClient http, ILogger logger)
        {
            accessToken = null;
            client_id = configuration["ProcessPlayerStatsAZF:client_id"];
            client_secret = configuration["AZF:StatsScraper:ClientSecret:Primary"];
            oauthEndpoint = configuration["OAuth:Domain"];
            semaphore = new Semaphore(1, 1);
            this.http = http;
            this.logger = logger;
        }

        public async Task<string> ObtainAccessTokenAsync()
        {
            // If we already got a token return it
            if(accessToken != null)
            {
                return accessToken;
            }

            // Lock to retrieve a token
            try
            {
                semaphore.WaitOne();
                // If 2 threads hit the lock the first gets token then the second should return what the 1st thread got
                if (accessToken != null)
                {
                    return accessToken;
                }

                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("grant_type", grant_type);
                request.AddParameter("client_id", client_id);
                request.AddParameter("client_secret", client_secret);
                request.AddParameter("scope", scope);

                var authDto = await http.ExecuteRequestAsync<OAuthResponseDto>(oauthEndpoint, request);
                accessToken = authDto.access_token;
                return accessToken;
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
