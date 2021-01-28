using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using ProcessPlayerStats.Clients;
using ProcessPlayerStats.Dtos.Response;

namespace ProcessPlayerStats
{
    public class FunctionTriggers
    {
        private readonly IHandler handler;
        private readonly IRocketClient rocketClient;

        public FunctionTriggers(IHandler handler, IRocketClient rocketClient)
        {
            this.handler = handler;
            this.rocketClient = rocketClient;
        }

        [FunctionName("Orchestrator")]
        public async Task<List<string>> RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();
            var getUsersTask = rocketClient.GetAllPlayersAsync();
            getUsersTask.Wait();
            var users = getUsersTask.Result;
            var tasks = new List<Task<string>>();

            foreach (var user in users)
            {
                tasks.Add(context.CallActivityAsync<string>("ProcessUser", user));
            }
            await Task.WhenAll(tasks.ToArray());

            foreach (var task in tasks)
            {
                outputs.Add(task.Result);
            }
            return outputs;
        }

        [FunctionName("ProcessUser")]
        public async Task<string> SayHello([ActivityTrigger] PlayerResponseDto player)
        {
            //logger.LogInformation($"Saying hello to {player.UserName}.");
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

        //[FunctionName("Orchestrator_HttpStart")]
        //public async Task<HttpResponseMessage> TimerStart(
        //    [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
        //    [DurableClient] IDurableOrchestrationClient starter,
        //    ILogger log)
        //{
        //    // Function input comes from the request content.
        //    string instanceId = await starter.StartNewAsync("Orchestrator", null);

        //    log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

        //    return starter.CreateCheckStatusResponse(req, instanceId);
        //}
    }
}