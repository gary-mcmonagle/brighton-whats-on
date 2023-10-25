using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Domain;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace Functions.Activities.Parsers;

public class HopeAndRuinParser
{
    [FunctionName($"{nameof(Venue.HopeAndRuin)}_ParserActivity")]
    public List<EventModel> SayHello([ActivityTrigger] IDurableActivityContext context)
    {
        var input = context.GetInput<RawScraperDto>();
        var data = Parse(input);
        return data.ToList();
    }

    private IEnumerable<EventModel> Parse(RawScraperDto rawScraperDto)
    {
        var convertToString = JsonConvert.SerializeObject(rawScraperDto.Data) as string;
        var data = JsonConvert.DeserializeObject<List<RawHopeAndRuin>>(convertToString);

        return data.Where(x => ParseFirstDate(x.Dates) != null).Select(x => new EventModel
        {
            Venue = Venue.HopeAndRuin,
            Name = x.Title,
            Date = ParseFirstDate(x.Dates) ?? DateTimeOffset.MinValue
        });
    }

    public static DateTimeOffset? ParseFirstDate(string input)
    {
        // Define the custom format string
        input = input.Replace("st", "").Replace("nd", "").Replace("rd", "").Replace("th", "");
        string customFormat = "dd MMMM yyyy - h:mm tt";

        // Create a CultureInfo to handle the month names
        CultureInfo cultureInfo = new CultureInfo("en-US");

        // Parse the input string using the custom format and culture info
        var gotDate = DateTime.TryParseExact(input, customFormat,
       System.Globalization.CultureInfo.InvariantCulture,
       System.Globalization.DateTimeStyles.None, out var startDate);

        return startDate;
    }

    private record RawHopeAndRuin
    {
        public string Title { get; init; }
        public string Dates { get; init; }
    }
}