using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Domain;
using Functions.Extensions;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Functions.Scrapers;

public class BrightonDomeScraper
{
    [FunctionName("Scraper_BrightonDome_web")]
    public async Task<ActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "scrape/bd")] HttpRequest req,
            [Table("events")] TableClient tableClient,

        [Blob($"whatson/scrapes/{nameof(Venue.BrightonDome)}/raw.json", FileAccess.Write, Connection = "AzureWebJobsStorage")] Stream rawStream,
        ILogger log)
    {
        tableClient.CreateIfNotExists();
        var dto = await GetData();
        rawStream.Write(System.Text.Encoding.UTF8.GetBytes(
            JsonConvert.SerializeObject(dto, Formatting.Indented)));
        return new NoContentResult();
    }



    private async Task<RawScraperDto> GetData()
    {
        var html = @"https://brightondome.org/whats_on/?offset=0&limit=500";

        HtmlWeb web = new HtmlWeb();

        var htmlDoc = await web.LoadFromWebAsync(html);
        var indicator = "window.__PRELOADED_STATE__=";
        var all = htmlDoc.DocumentNode.SelectNodes("/html/body/div[2]/div/div/div/div[3]/div/div/div/section").ToList().Select(section =>
        {
            return new
            {
                Title = section.SelectSingleNode("div[2]/div[1]/hgroup/h1").InnerText.RemoveDodgyChars(),
                Dates = section.SelectSingleNode("div[2]/div[1]/hgroup/h2/dd[1]").InnerText.RemoveDodgyChars(),
            };
        });
        return new RawScraperDto
        {
            Venue = Venue.BrightonDome,
            Data = all
        };
    }
}
