namespace BasketballAnalytics.Application.Features.Teams.Dtos;

public class TeamStatsDto
{
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int PlayerCount { get; set; }
    public double AverageHeightCm { get; set; }
    public double AverageWeightKg { get; set; }
    public Dictionary<string, int> PositionBreakdown { get; set; } = new();
}
