using System.Linq;
using System.Threading.Tasks;
using Domain;
using Functions.Extensions;
using HtmlAgilityPack;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Functions.Activities.Scrapers;

public class HopeAndRuinScraper
{
    [FunctionName($"{nameof(Venue.HopeAndRuin)}_ScraperActivity")]
    public async Task<RawScraperDto> SayHello([ActivityTrigger] IDurableActivityContext context)
    {
        return await GetData();
    }

    private async Task<RawScraperDto> GetData()
    {
        var html = @"https://www.hope.pub/events/";

        HtmlWeb web = new HtmlWeb();

        var htmlDoc = await web.LoadFromWebAsync(html);
        var all = htmlDoc.DocumentNode.SelectNodes("//*[@id=\"events-list-alternate-0\"]/div/div[2]/div/div/div/div/div[2]/div[1]").ToList().Select(section =>
        {
            return new
            {
                Title = section.SelectSingleNode("div/h3/a").InnerText.RemoveDodgyChars(),
                Dates = section.SelectSingleNode("div/span[1]").InnerText.RemoveDodgyChars(),
            };
        });
        return new RawScraperDto
        {
            Venue = Venue.HopeAndRuin,
            Data = all
        };
    }
}
