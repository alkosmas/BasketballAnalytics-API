using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasketballAnalytics.Application.Common.Interfaces;
using BasketballAnalytics.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace BasketballAnalytics.Persistence.DbContext
{
    public class ApplicationDbContext:
    Microsoft.EntityFrameworkCore.DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

        public DbSet<Team> Teams => Set<Team>();

        public DbSet<User> Users => Set<User>();
    }
}