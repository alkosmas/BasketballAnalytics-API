using BasketballAnalytics.Application.Common.Interfaces;
using BasketballAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BasketballAnalytics.Persistence.DbContext;

public class ApplicationDbContext : Microsoft.EntityFrameworkCore.DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Team> Teams => Set<Team>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Player> Players => Set<Player>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Player>()
            .HasOne(p => p.Team)
            .WithMany(t => t.Players)
            .HasForeignKey(p => p.TeamId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Player>()
            .HasIndex(p => p.TeamId);
    }
}
