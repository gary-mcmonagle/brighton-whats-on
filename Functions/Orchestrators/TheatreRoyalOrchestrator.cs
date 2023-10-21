using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Domain;
using Functions.Activities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Functions.Orchestrators;

public class TheatreRoyalOrchestrator
{
    [FunctionName("TheatreRoyalOrchestrator")]
    public async Task<List<string>> Run([OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var outputs = new List<string>();
        var scrape = await context.CallActivityAsync<RawScraperDto>($"{nameof(Venue.TheatreRoyal)}_ScraperActivity", "theatre royal");
        var parse = await context.CallActivityAsync<List<EventModel>>($"{nameof(Venue.TheatreRoyal)}_ParserActivity", scrape);
        await context.CallActivityAsync("ChangeLoggerActivity", new ChangeLoggerParams(parse, nameof(Venue.TheatreRoyal)));
        return outputs;
    }

    [FunctionName("TheatreRoyalOrchestrator_HttpStart")]
    public static async Task<HttpResponseMessage> HttpStart(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
    [DurableClient] IDurableOrchestrationClient starter,
    ILogger log)
    {
        // Function input comes from the request content.
        string instanceId = await starter.StartNewAsync("TheatreRoyalOrchestrator", null);

        log.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

        return starter.CreateCheckStatusResponse(req, instanceId);
    }
}
