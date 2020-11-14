using Azure.Identity;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using ProcessPlayerStats.Clients;
using RestSharp;
using System;

[assembly: FunctionsStartup(typeof(ProcessPlayerStats.StartUp.Startup))]

namespace ProcessPlayerStats.StartUp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Configuration Providers
            var configBuilder = new ConfigurationBuilder();
            configBuilder
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables(); // Machine Name etc

            var builtConfig = configBuilder.Build();
            var creds = new DefaultAzureCredential();

            configBuilder.AddAzureAppConfiguration(options => options
                .Connect(new Uri(builtConfig["AppConfig:Endpoint"]), creds)
                .ConfigureKeyVault(kv => kv.SetCredential(creds))
                .Select(KeyFilter.Any, builtConfig["Environment"]));
            var config = configBuilder.Build();
            
            // Services
            builder.Services.AddSingleton<IConfiguration>(config);
            builder.Services.AddSingleton<IRestClient, RestClient>();
            builder.Services.AddSingleton<IHttpClient, HttpClient>();
            builder.Services.AddSingleton<IAuth0Client, Auth0Client>();
            builder.Services.AddSingleton<IRocketClient, RocketClient>();
            builder.Services.AddLogging();
        }        
    }
}
