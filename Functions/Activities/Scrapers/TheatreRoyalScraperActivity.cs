using System.Linq;
using System.Threading.Tasks;
using Domain;
using Functions.Extensions;
using HtmlAgilityPack;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace Functions.Activities.Scrapers;

public class TheatreRoyalScraperActivity
{
    [FunctionName($"{nameof(Venue.TheatreRoyal)}_ScraperActivity")]
    public async Task<RawScraperDto> SayHello([ActivityTrigger] IDurableActivityContext context)
    {
        return await GetData();
    }

    private async Task<RawScraperDto> GetData()
    {
        var html = @"https://www.atgtickets.com/venues/theatre-royal-brighton/";

        HtmlWeb web = new HtmlWeb();

        var htmlDoc = await web.LoadFromWebAsync(html);
        var indicator = "window.__PRELOADED_STATE__=";
        var all = htmlDoc.DocumentNode.Descendants("script").ToList();
        var desc = htmlDoc.DocumentNode.Descendants("script").Where(x => x.InnerText.Contains(indicator)).FirstOrDefault();
        if (desc == null)
        {
            // throw
        }
        var json = desc.InnerText.Replace(indicator, "");
        return new RawScraperDto
        {
            Venue = Venue.TheatreRoyal,
            Data = JsonConvert.DeserializeObject<dynamic>(json)
        };
    }
}
