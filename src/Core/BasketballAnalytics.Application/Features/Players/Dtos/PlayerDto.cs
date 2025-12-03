namespace BasketballAnalytics.Application.Features.Players.Dtos;

public class PlayerDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public int HeightCm { get; set; }
    public int WeightKg { get; set; }
    public string Position { get; set; } = string.Empty;
    public string JerseyNumber { get; set; } = string.Empty;
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
}
