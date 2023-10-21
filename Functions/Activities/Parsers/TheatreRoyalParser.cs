using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Functions.Extensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace Functions.Activities.Parsers;

public class TheatreRoyalParser
{
    [FunctionName($"{nameof(Venue.TheatreRoyal)}_ParserActivity")]
    public List<EventModel> SayHello([ActivityTrigger] IDurableActivityContext context)
    {
        var input = context.GetInput<RawScraperDto>();
        var data = Parse(input);
        return data.ToList();
    }

    private IEnumerable<EventModel> Parse(RawScraperDto rawScraperDto)
    {
        var convertToString = JsonConvert.SerializeObject(rawScraperDto.Data) as string;
        var data = JsonConvert.DeserializeObject<RawTheatreRoyal>(convertToString);

        return data.Hub.VenueHubData.ShowCards.Select(x => new EventModel
        {
            Venue = Venue.TheatreRoyal,
            Name = x.Title.RemoveDodgyChars(),
            Date = ParseFirstDate(x.SalePeriod) ?? DateTimeOffset.MinValue
        });
    }


    public static DateTimeOffset? ParseFirstDate(string input)
    {
        string[] formats = { "ddd d MMM yyyy", "ddd d MMM - ddd d MMM yyyy" };

        string[] dateStrings = input.Split('-');
        foreach (string dateString in dateStrings)
        {
            if (DateTime.TryParseExact(dateString.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                var d = result;
                DateTime.SpecifyKind(d, DateTimeKind.Utc);
                return d;
            }
        }

        return null; // Return null if no valid date is found
    }

    private record RawTheatreRoyal
    {
        [JsonProperty("venueHub")]
        public VenueHub Hub { get; init; } = new();

        public record VenueHub
        {
            public VenueHubData VenueHubData { get; init; } = new();
        }
        public record VenueHubData
        {
            public List<ShowCard> ShowCards { get; init; } = new();
        }
        public record ShowCard
        {
            public string Title { get; init; } = string.Empty;
            public string SalePeriod { get; init; } = string.Empty;
        }
    }

}
