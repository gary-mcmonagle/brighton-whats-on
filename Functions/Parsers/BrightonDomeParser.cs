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

public class BrightonDomeParser
{
    [FunctionName("Parser_BrightonDome")]
    public void Run(
        [BlobTrigger($"whatson/scrapes/{nameof(Venue.BrightonDome)}/raw.json")] string myBlob,
        [Blob($"whatson/scrapes/{nameof(Venue.BrightonDome)}/latest.json", FileAccess.Write, Connection = "AzureWebJobsStorage")] Stream rawStream,
         ILogger log)
    {
        var events = Parse(JsonConvert.DeserializeObject<RawScraperDto>(myBlob.ToString()));
        var model = new VenueEventsModel
        {
            Venue = Venue.BrightonDome,
            Events = events
        };
        rawStream.Write(System.Text.Encoding.UTF8.GetBytes(
            JsonConvert.SerializeObject(model, Formatting.Indented)));
    }
    private IEnumerable<EventModel> Parse(RawScraperDto rawScraperDto)
    {
        var convertToString = JsonConvert.SerializeObject(rawScraperDto.Data) as string;
        var data = JsonConvert.DeserializeObject<List<RawBrightonDome>>(convertToString);



        return data.Where(x => ParseFirstDate(x.Dates) != null).Select(x => new EventModel
        {
            Venue = Venue.BrightonDome,
            Name = x.Title,
            Date = ParseFirstDate(x.Dates) ?? DateTimeOffset.MinValue
        });
    }

    public static DateTimeOffset? ParseFirstDate(string input)
    {
        // Define custom date and time formats to match the provided date formats.
        string[] customDateFormats = new string[]
        {
            "ddd d MMM yyyy",
            "ddd d MMM - d MMM yyyy",
            "ddd d MMMM yyyy",
            "ddd d MMMM yyyy",
            "ddd d  MMM yyyy",
            "ddd d  MMM - d MMM yyyy",
            "ddd d  MMMM yyyy",
            "ddd d  MMMM yyyy",
        };

        // Split the input by '-' to handle date ranges.
        string[] dateParts = input.Split('-');

        foreach (string datePart in dateParts)
        {
            string trimmedDatePart = datePart.Trim();
            DateTime parsedDate;

            if (DateTime.TryParseExact(trimmedDatePart, customDateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                return parsedDate;
            }
        }

        return null;
    }


    private record RawBrightonDome
    {
        public string Title { get; init; }
        public string Dates { get; init; }
    }
}
