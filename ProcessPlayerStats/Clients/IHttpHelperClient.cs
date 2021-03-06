﻿using RestSharp;
using System.Threading.Tasks;

namespace ProcessPlayerStats.Clients
{
    public interface IHttpHelperClient
    {
        Task ExecuteRequestAsync(string url, IRestRequest restRequest);
        Task<T> ExecuteRequestAsync<T>(string url, IRestRequest restRequest);
    }
}
