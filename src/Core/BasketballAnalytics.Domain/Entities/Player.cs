namespace BasketballAnalytics.Domain.Entities;

public class Player
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int HeightCm { get; set; }
    public int WeightKg { get; set; }
    public Position Position { get; set; }
    public string JerseyNumber { get; set; } = string.Empty;
    public Guid TeamId { get; set; }
    public Team Team { get; set; } = null!;
}

public enum Position
{
    PointGuard = 1,
    ShootingGuard = 2,
    SmallForward = 3,
    PowerForward = 4,
    Center = 5
}
