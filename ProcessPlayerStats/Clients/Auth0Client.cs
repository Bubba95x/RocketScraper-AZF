using Microsoft.Extensions.Configuration;
using ProcessPlayerStats.Models;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace ProcessPlayerStats.Clients
{
    public class Auth0Client : IAuthClient
    {
        private readonly IHttpClient httpClient;

        private readonly string audience;
        private readonly string auth0Url;
        private readonly string client_id;
        private readonly string client_secret;
        private DateTime Expiration;
        private string Token;

        public Auth0Client(IConfiguration configuration, IHttpClient httpClient)
        {
            this.httpClient = httpClient;

            auth0Url = configuration["Auth0:Token"];
            client_id = configuration["ProcessPlayerStatsAZF:client_id"];
            client_secret = configuration["ProcessPlayerStatsAZF:client_secret"];
            audience = configuration["Auth0:Audience"];

            Token = null;
            Expiration = DateTime.UtcNow;
        }

        public async Task<string> ObtainAccessTokenAsync()
        {
            if(Token == null || DateTime.UtcNow < Expiration)
            {
                var request = new RestRequest(Method.POST);
                request.AddParameter("client_id", client_id);
                request.AddParameter("client_secret", client_secret);
                request.AddParameter("audience", audience);
                request.AddParameter("grant_type", "client_credentials");

                var response = await httpClient.ExecuteRequestAsync<Auth0Dto>(auth0Url, request);
                Token = response.access_token;
                Expiration = DateTime.UtcNow + TimeSpan.FromMinutes(30);
            }
            
            return Token;
        }
    }
}
