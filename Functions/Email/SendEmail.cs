using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Domain;
using Email;
using Functions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Functions.Email;

public class SendEmail
{

    private readonly IEmailService _emailService;

    public SendEmail(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [FunctionName("SendEmail_Http")]
    public async Task<ActionResult> SendEmailHttp(
    [HttpTrigger(AuthorizationLevel.Function, "post", Route = "sendEmail")] HttpRequest req, [Table("events")] TableClient tableClient,
    ILogger log)
    {
        await SendEmailAsync(tableClient);
        return new NoContentResult();
    }

    private async Task SendEmailAsync(TableClient tableClient)
    {
        var queryResults = tableClient.QueryAsync<EventTableModel>();
        var events = new List<EventTableModel>();
        await foreach (var entity in queryResults)
        {
            events.Add(entity);
        };
        var email = new NewEventsEmailModel
        {
            To = Environment.GetEnvironmentVariable("EMAIL_TO") ?? string.Empty,
            Subject = "What's on in Brighton",
            Events = events.Select(x => x as EventModel).ToList(),
        };
        await _emailService.SendNewEventsEmail(email);
    }
}