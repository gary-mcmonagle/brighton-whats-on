using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace Functions.Activities.Events;

public class SetEventsToAlerted
{
    [FunctionName($"SetAlertableEventsActivity")]
    public async Task<bool> GetEvents([ActivityTrigger] IDurableActivityContext context,
        [Table("events")] TableClient tableClient,
        ILogger log
)
    {
        var input = context.GetInput<List<EventTableModel>>();
        foreach (var woEvent in input)
        {
            woEvent.Alerted = true;
            await tableClient.UpsertEntityAsync(woEvent);
        }
        return true;
    }
}
