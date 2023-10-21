using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Functions.Activities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Functions.Orchestrators;

public class EventSubOrchestrator
{
    [FunctionName("EventSubOrchestrator")]
    public async Task<List<string>> Run([OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        Venue venue = context.GetInput<Venue>();
        var venueName = venue.convertToString();

        var outputs = new List<string>();
        var scrape = await context.CallActivityAsync<RawScraperDto>($"{venueName}_ScraperActivity", "theatre royal");
        var parse = await context.CallActivityAsync<List<EventModel>>($"{venueName}_ParserActivity", scrape);
        await context.CallActivityAsync("ChangeLoggerActivity", new ChangeLoggerParams(parse, venueName));
        return outputs;
    }

}
public static class EnumExtensions
{
    public static string convertToString(this Venue eff)
    {
        return Enum.GetName(eff.GetType(), eff);
    }

}

