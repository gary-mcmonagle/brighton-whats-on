namespace Domain;

public record RawScraperDto
{
    public Venue Venue { get; init; }
    public dynamic Data { get; init; }
}
