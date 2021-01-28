using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RestSharp;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProcessPlayerStats.Clients
{
    public class HttpHelperClient : IHttpHelperClient
    {
        public static int RetryCount = 3;
        public static int RetryDelay = 1;

        private readonly IRestClient restClient;
        private readonly ILogger logger;

        private readonly AsyncRetryPolicy<IRestResponse> retryPolicy;

        /// <summary>
        /// Creates an http client that handles http responses and deserialization
        /// </summary>
        /// <param name="restClient"></param>
        /// <param name="logger"></param>
        /// <param name="retryCount">Number of times the client should retry the request</param>
        /// <param name="retryDelay">The delay in seconds between request retries</param>
        public HttpHelperClient(IRestClient restClient, ILogger logger)
        {
            this.restClient = restClient;
            this.logger = logger;

            retryPolicy = Policy.HandleResult<IRestResponse>(
                x => x.StatusCode != HttpStatusCode.OK
                    && x.StatusCode != HttpStatusCode.NoContent
                    && x.StatusCode != HttpStatusCode.Created
                    && x.StatusCode != HttpStatusCode.Accepted
                ).WaitAndRetryAsync(RetryCount, attempt => TimeSpan.FromSeconds(RetryDelay));
        }

        public async Task ExecuteRequestAsync(string url, IRestRequest restRequest)
        {
            restClient.BaseUrl = new Uri(url);
            var response = await retryPolicy.ExecuteAsync(async () => await restClient.ExecuteAsync(restRequest));
            HttpStatusCode statusCode = response.StatusCode;

            if (statusCode != HttpStatusCode.OK
               && statusCode != HttpStatusCode.Created
               && statusCode != HttpStatusCode.Accepted
               && statusCode != HttpStatusCode.NoContent)
            {
                //logger.LogError($"{nameof(HttpHelperClient)}.{nameof(ExecuteRequestAsync)} No Body - Failed with status code {response.StatusCode}.");
                throw new HttpRequestException($"Request to {url} failed!");
            }
        }

        public async Task<T> ExecuteRequestAsync<T>(string url, IRestRequest restRequest)
        {
            restClient.BaseUrl = new Uri(url);
            var response = await retryPolicy.ExecuteAsync(async () => await restClient.ExecuteAsync(restRequest));
            HttpStatusCode statusCode = response.StatusCode;

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return default;
            }
            else if (statusCode != HttpStatusCode.OK
                && statusCode != HttpStatusCode.Created
                && statusCode != HttpStatusCode.Accepted
                && statusCode != HttpStatusCode.NoContent)
            {
                //logger.LogError($"{nameof(HttpHelperClient)}.{nameof(ExecuteRequestAsync)} No Body - Failed with status code {response.StatusCode}.");
                throw new HttpRequestException($"Request to {url} failed!");
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(response.Content);
            }
            catch(Exception ex)
            {
                logger.LogError($"{nameof(HttpHelperClient)}.{nameof(ExecuteRequestAsync)} - Failed to deserialize object.", ex);
                throw ex;
            }
        }
    }
}
