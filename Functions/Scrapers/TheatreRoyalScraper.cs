using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Functions.Scrapers;

public class TheatreRoyalScraper
{
    [FunctionName("Scraper_TheatreRoyal_web")]
    public async Task<ActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "scrape/tr")] HttpRequest req,
        [Blob("whatson/scrapes/theatre_royal/raw.json", FileAccess.Write, Connection = "AzureWebJobsStorage")] Stream rawStream,
        ILogger log)
    {
        var dto = await GetData();
        rawStream.Write(System.Text.Encoding.UTF8.GetBytes(
            JsonConvert.SerializeObject(dto, Formatting.Indented)));
        return new NoContentResult();
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
