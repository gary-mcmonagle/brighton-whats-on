namespace Domain;

public record EventModel
{
    public Venue Venue { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTimeOffset Date { get; init; }
}
