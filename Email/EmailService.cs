using Azure;
using Azure.Communication.Email;
using Domain;
using HandlebarsDotNet;


namespace Email;

public class EmailService : IEmailService
{
    public Task SendNewEventsEmail(NewEventsEmailModel email)
    {
        string source =
            @"<div class=""entry"">
            <h1>{{title}}</h1>
            <div class=""body"">
                <h2>New Events</h2>
                {{#each events}}
                <ul>
                    <li>{{Name}}</li>
                    <li>{{Venue}}</li>
                </ul>
                {{/each}}
            </div>
            </div>";
        var template = Handlebars.Compile(source);

        var data = new
        {
            title = "Event updates",
            events = email.Events
        };

        var result = template(data);
        return SendEmail(email, result);
    }

    private async Task SendEmail(EmailModel email, string body)
    {
        string connectionString = Environment.GetEnvironmentVariable("EMAIL_CONNECTION_STRING") ?? string.Empty;
        var emailClient = new EmailClient(connectionString);
        await emailClient.SendAsync(
            WaitUntil.Completed,
            new EmailMessage(
                Environment.GetEnvironmentVariable("EMAIL_FROM"),
                email.To,
                new EmailContent(email.Subject) { Html = body }),
            default);
    }
}
