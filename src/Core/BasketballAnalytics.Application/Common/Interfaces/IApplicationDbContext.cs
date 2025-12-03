using BasketballAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BasketballAnalytics.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Team> Teams { get; }
    DbSet<User> Users { get; }
    DbSet<Player> Players { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
