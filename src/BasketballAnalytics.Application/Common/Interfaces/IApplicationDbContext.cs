using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasketballAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BasketballAnalytics.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Team> Teams { get; }
        DbSet<User> Users { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
