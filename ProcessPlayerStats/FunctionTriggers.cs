using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProcessPlayerStats.Clients;
using ProcessPlayerStats.Models;

namespace ProcessPlayerStats
{
    public class FunctionTriggers
    {
        private readonly IConfiguration config;
        private readonly IHandler handler;
        private readonly IRocketClient rocketClient;

        public FunctionTriggers(IConfiguration config, IHandler handler, IRocketClient rocketClient)
        {
            this.config = config;
            this.handler = handler;
            this.rocketClient = rocketClient;
        }

        [FunctionName("Orchestrator")]
        public async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();
            var getUsersTask = rocketClient.GetAllPlayersAsync();
            getUsersTask.Wait();
            var users = getUsersTask.Result;
            var tasks = new List<Task<string>>();

            foreach(var user in users)
            {
                tasks.Add(context.CallActivityAsync<string>("ProcessUser", user));
            }
            await Task.WhenAll(tasks.ToArray());
            
            foreach(var task in tasks)
            {
                outputs.Add(task.Result);
            }
            return outputs;
        }

        [FunctionName("ProcessUser")]
        public async Task<string> SayHello([ActivityTrigger] PlayerDto player, ILogger log)
        {
            log.LogInformation($"Saying hello to {player.UserName}.");
            var result = await handler.ProcessEventAsync(player);
            return $"Hello {result}!";
        }

        [FunctionName("Orchestrator_HttpStart")]
        public async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("Orchestrator", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}