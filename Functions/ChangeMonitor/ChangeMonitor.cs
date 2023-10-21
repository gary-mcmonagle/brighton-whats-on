using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Domain;
using Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Functions.ChangeMonitor;

public class ChangeMonitor
{
    [FunctionName("ChangeMonitor")]
    public async Task Run(
    [BlobTrigger("whatson/scrapes/{venueName}/latest.json")] string latestJson,
    [Table("events")] TableClient tableClient,
    string venueName,
     ILogger log)
    {
        var queryResults = tableClient.QueryAsync<EventTableModel>(filter: $"Venue eq '{venueName}'");
        var existingIds = new List<string>();
        await foreach (var entity in queryResults)
        {
            existingIds.Add(entity.EventId);
        }
        var woevents = JsonConvert.DeserializeObject<VenueEventsModel>(latestJson).Events;
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
