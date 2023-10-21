using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Domain;
using Functions.Activities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Functions.Orchestrators;

public class AllEventsOrchestrator
{
    [FunctionName("AllEventsOrchestrator")]
    public async Task<List<string>> Run([OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var outputs = new List<string>();
        var tasks = new List<Venue> { Venue.TheatreRoyal, Venue.BrightonDome }.Select(x => context.CallSubOrchestratorAsync("EventSubOrchestrator", x));
        await Task.WhenAll(tasks);
        return outputs;
    }

    [FunctionName("AllEvents_HttpStart")]
    public static async Task<HttpResponseMessage> HttpStart(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
    [DurableClient] IDurableOrchestrationClient starter,
    ILogger log)
    {
        // Function input comes from the request content.
        string instanceId = await starter.StartNewAsync("AllEventsOrchestrator", null);

        log.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

        return starter.CreateCheckStatusResponse(req, instanceId);
    }
}
