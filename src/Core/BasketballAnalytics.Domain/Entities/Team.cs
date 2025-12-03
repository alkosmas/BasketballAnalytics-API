namespace BasketballAnalytics.Domain.Entities;

public class Team : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    
    public ICollection<Player> Players { get; set; } = new List<Player>();
}
