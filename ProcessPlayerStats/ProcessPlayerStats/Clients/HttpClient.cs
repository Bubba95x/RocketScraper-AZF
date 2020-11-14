using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace ProcessPlayerStats.Clients
{
    public class HttpClient : IHttpClient
    {
        private readonly IRestClient restClient;
        private readonly ILogger<HttpClient> logger;

        public HttpClient(IRestClient restClient, ILoggerFactory loggerFactory)
        {
            this.restClient = restClient;
            logger = loggerFactory.CreateLogger<HttpClient>();
        }

        public async Task ExecuteRequestAsync(string url, IRestRequest restRequest)
        {
            restClient.BaseUrl = new Uri(url);
            var retryPolicy = Policy.HandleResult<IRestResponse>(x => !x.IsSuccessful)
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

            var response = await retryPolicy.ExecuteAsync(async () => await restClient.ExecuteAsync(restRequest));

            if (!response.IsSuccessful)
            {
                logger.LogError($"{nameof(HttpClient)}.{nameof(ExecuteRequestAsync)} No Body - Failed with status code {response.StatusCode}.");
                throw new Exception($"Request to {url} failed!");
            }
        }

        public async Task<T> ExecuteRequestAsync<T>(string url, IRestRequest restRequest)
        {
            restClient.BaseUrl = new Uri(url);
            var retryPolicy = Policy.HandleResult<IRestResponse>(x => !x.IsSuccessful)
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

            var response = await retryPolicy.ExecuteAsync(async () => await restClient.ExecuteAsync(restRequest));

            if (!response.IsSuccessful)
            {
                logger.LogError($"{nameof(HttpClient)}.{nameof(ExecuteRequestAsync)} No Body - Failed with status code {response.StatusCode}.");
                throw new Exception($"Request to {url} failed!");
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(response.Content);
            }
            catch(Exception ex)
            {
                logger.LogError($"{nameof(HttpClient)}.{nameof(ExecuteRequestAsync)} - Failed to deserialize object.", ex);
                throw ex;
            }
        }
    }
}
