namespace BasketballAnalytics.Application.Common.Events;

public record PlayerCreatedEvent
{
    public Guid PlayerId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public Guid TeamId { get; init; }
    public DateTime CreatedAt { get; init; }
}
