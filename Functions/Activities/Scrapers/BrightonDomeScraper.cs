using System.Linq;
using System.Threading.Tasks;
using Domain;
using Functions.Extensions;
using HtmlAgilityPack;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Functions.Activities.Scrapers;

public class BrightonDomeScraper
{
    [FunctionName($"{nameof(Venue.BrightonDome)}_ScraperActivity")]
    public async Task<RawScraperDto> SayHello([ActivityTrigger] IDurableActivityContext context)
    {
        return await GetData();
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
