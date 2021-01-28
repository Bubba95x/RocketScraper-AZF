using Microsoft.Extensions.Configuration;
using ProcessPlayerStats.Dtos.Response;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace ProcessPlayerStats.Clients
{
    [Obsolete]
    public class Auth0Client : IAuthClient
    {
        private readonly IHttpHelperClient httpClient;

        private readonly string audience;
        private readonly string oauthEndpoint;
        private readonly string client_id;
        private readonly string client_secret;
        private DateTime Expiration;
        private string Token;

        public Auth0Client(IConfiguration configuration, IHttpHelperClient httpClient)
        {
            this.httpClient = httpClient;

            oauthEndpoint = configuration["OAuth:Domain"];
            client_id = configuration["ProcessPlayerStatsAZF:client_id"];
            client_secret = configuration["AZF:StatsScraper:ClientSecret:Primary"];
            audience = configuration["Oauth:Audience"];

            Token = null;
            Expiration = DateTime.UtcNow;
        }

        public async Task<string> ObtainAccessTokenAsync()
        {
            if(Token == null || DateTime.UtcNow < Expiration) // Not thread safe.  But this over now. Not sure I need the expiration here
            {
                var request = new RestRequest(Method.POST);
                request.AddParameter("client_id", client_id);
                request.AddParameter("client_secret", client_secret);
                request.AddParameter("audience", audience);
                request.AddParameter("grant_type", "client_credentials");

                var response = await httpClient.ExecuteRequestAsync<Auth0ResponseDto>(oauthEndpoint, request);
                Token = response.access_token;
                Expiration = DateTime.UtcNow + TimeSpan.FromMinutes(30);
            }
            
            return Token;
        }
    }
}
