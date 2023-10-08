using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Functions.ChangeMonitor;

public class ChangeMonitor
{
    [FunctionName("ChangeMonitor")]
    public void Run(
    [BlobTrigger("whatson/scrapes/{name}/latest.json")] string latestJson,
    string name,
     ILogger log)
    {
        log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {latestJson.Length} Bytes");
    }
}
