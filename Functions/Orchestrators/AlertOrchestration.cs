using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Domain;
using Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Functions.Orchestrators;

public class AlertOrchestration
{
    [FunctionName("AlertOrchestration")]
    public async Task<List<string>> Run([OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var outputs = new List<string>();
        var events = await context.CallActivityAsync<List<EventTableModel>>("GetAlertableEventsActivity", null);
        if (events.Any())
        {
            await context.CallActivityAsync<bool>("SendNewEventsEmailActivity", events);
            await context.CallActivityAsync<bool>("SetAlertableEventsActivity", events);
        }
        return outputs;
    }

    [FunctionName("AlertOrchestration_HttpStart")]
    public static async Task<HttpResponseMessage> HttpStart(
[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
[DurableClient] IDurableOrchestrationClient starter)
    {
        // Function input comes from the request content.
        string instanceId = await starter.StartNewAsync("AlertOrchestration", null);
        return starter.CreateCheckStatusResponse(req, instanceId);
    }
}
