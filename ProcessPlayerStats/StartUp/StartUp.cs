using Azure.Identity;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
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

            configBuilder.AddAzureKeyVault(new Uri(builtConfig["keyvaulturl"]), new DefaultAzureCredential());
            var config = configBuilder.Build();

            // Services
            builder.Services.AddLogging();
            builder.Services.AddDistributedRedisCache(x => x.Configuration = config["Redis:PrimaryConnectionString"]);
            builder.Services.AddSingleton<IConfiguration>(config);
            builder.Services.AddScoped<IRestClient, RestClient>();
            builder.Services.AddScoped<IHttpHelperClient, HttpHelperClient>();
            builder.Services.AddScoped<IAuthClient, IdentityServerAuthClient>();
            builder.Services.AddScoped<IRocketClient, RocketClientDecorator>();
            builder.Services.AddScoped<IRocketStatsClient, RocketStatsClient>();
            builder.Services.AddScoped<IHandler, ProcessPlayerHandler>();
        }        
    }
}
