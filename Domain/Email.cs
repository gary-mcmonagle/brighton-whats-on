namespace Domain;

public abstract record EmailModel
{
    public string Subject { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
}

public record NewEventsEmailModel : EmailModel
{
    public List<EventModel> Events { get; init; } = new();
}
