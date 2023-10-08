namespace Domain;

public record VenueEventsModel
{
    public IEnumerable<EventModel> Events { get; init; } = new List<EventModel>();
    public Venue Venue { get; init; }
}
