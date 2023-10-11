using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Functions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Functions.SetReminder;

public class SetToBeReminded
{
    [FunctionName("SetToBeReminded")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "setToBeReminded/{id}")] HttpRequest req,
            [Table("events")] TableClient tableClient,

        Guid id,
        ILogger log)
    {
        log.LogInformation(id.ToString());
        string name = req.Query["name"];
        var queryResults = tableClient.QueryAsync<EventTableModel>(filter: $"Id eq guid'{id}'");
        var results = new List<EventTableModel>();
        await foreach (var entity in queryResults)
        {
            results.Add(entity);
        }
        if (results.Count == 0)
        {
            return new NotFoundResult();
        }
        var result = results.First();
        result.Remind = true;
        tableClient.UpsertEntity(result);
        return new OkResult();
    }
}