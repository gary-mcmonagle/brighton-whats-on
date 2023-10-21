using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Domain;
using Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace Functions.Activities.Events;

public class GetAlertableEvents
{
    [FunctionName($"GetAlertableEventsActivity")]
    public async Task<List<EventTableModel>> GetEvents([ActivityTrigger] IDurableActivityContext context,
     [Table("events")] TableClient tableClient,
     ILogger log
)
    {
        var queryResults = tableClient.QueryAsync<EventTableModel>();
        var events = new List<EventTableModel>();
        await foreach (var entity in queryResults)
        {
            events.Add(entity);
        };
        var notAlreadySent = events.Where(e => !e.Alerted);
        var toRemind = notAlreadySent.Where(e => e.Remind).Where(e => e.Date.Date > DateTimeOffset.UtcNow.AddDays(-7).Date);
        var combined = notAlreadySent.Where(e => !e.Remind).Concat(toRemind).OrderBy(e => e.Date);
        return combined.ToList();
    }
}
