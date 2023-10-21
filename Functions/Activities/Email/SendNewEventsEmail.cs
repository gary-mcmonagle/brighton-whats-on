using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Email;
using Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace Functions.Activities.Email;

public class SendNewEventsEmail
{
    private readonly IEmailService _emailService;

    public SendNewEventsEmail(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [FunctionName($"SendNewEventsEmailActivity")]
    public async Task<bool> NewEventsEmail([ActivityTrigger] IDurableActivityContext context,
        ILogger log)
    {
        var input = context.GetInput<List<EventTableModel>>();
        var email = new NewEventsEmailModel
        {
            To = Environment.GetEnvironmentVariable("EMAIL_TO") ?? string.Empty,
            Subject = "What's on in Brighton",
            Events = input.Select(x => x as EventModel).ToList(),
        };
        await _emailService.SendNewEventsEmail(email);
        return true;
    }
}
