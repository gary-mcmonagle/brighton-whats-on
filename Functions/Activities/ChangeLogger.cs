using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Domain;
using Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace Functions.Activities;

public class ChangeLogger
{
    [FunctionName($"ChangeLoggerActivity")]
    public async Task LogChange([ActivityTrigger] IDurableActivityContext context,
     [Table("events")] TableClient tableClient,
     ILogger log
)
    {
        tableClient.CreateIfNotExists();
        var input = context.GetInput<ChangeLoggerParams>();

        var events = input.Events;
        var venueName = input.VenueName;

        var queryResults = tableClient.QueryAsync<EventTableModel>(filter: $"Venue eq '{venueName}'");
        var existingIds = new List<string>();
        await foreach (var entity in queryResults)
        {
            existingIds.Add(entity.EventId);
        }
        var woevents = events;
        foreach (var woevent in woevents)
        {
            var strongEntity = new EventTableModel(woevent)
            {
                PartitionKey = "woevent",
                RowKey = Guid.NewGuid().ToString(),
                Timestamp = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
            };
            if (!existingIds.Contains(strongEntity.EventId))
            {
                log.LogInformation($"New event found: {woevent.Name}");
                tableClient.UpsertEntity(strongEntity);
            }
        }
    }

}

public record ChangeLoggerParams(List<EventModel> Events, string VenueName);
