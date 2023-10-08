using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Domain;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Functions.Parsers;

public class TheatreRoyalParser
{
    [FunctionName("Parser_TheatreRoyal")]
    public void Run(
        [BlobTrigger("scrapes/theatre_royal/raw.json")] string myBlob,
        [Blob("scrapes/theatre_royal/latest.json", FileAccess.Write, Connection = "AzureWebJobsStorage")] Stream rawStream,
         ILogger log)
    {
        var events = Parse(JsonConvert.DeserializeObject<RawScraperDto>(myBlob.ToString()));
        rawStream.Write(System.Text.Encoding.UTF8.GetBytes(
            JsonConvert.SerializeObject(events, Formatting.Indented)));
    }
    private IEnumerable<EventModel> Parse(RawScraperDto rawScraperDto)
    {
        var convertToString = JsonConvert.SerializeObject(rawScraperDto.Data) as string;
        var data = JsonConvert.DeserializeObject<RawTheatreRoyal>(convertToString);

        return data.Hub.VenueHubData.ShowCards.Select(x => new EventModel
        {
            Venue = Venue.TheatreRoyal,
            Name = x.Title,
            Date = ParseFirstDate(x.SalePeriod) ?? DateTime.MinValue
        });
    }


    public static DateTime? ParseFirstDate(string input)
    {
        string[] formats = { "ddd d MMM yyyy", "ddd d MMM - ddd d MMM yyyy" };

        string[] dateStrings = input.Split('-');
        foreach (string dateString in dateStrings)
        {
            if (DateTime.TryParseExact(dateString.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return result;
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
