namespace Domain;

public record EventModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Venue Venue { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTimeOffset Date { get; init; }

    public bool Alerted { get; set; } = false;
    public bool Remind { get; set; } = false;
}
