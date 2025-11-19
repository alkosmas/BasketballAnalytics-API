using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasketballAnalytics.IntegrationTests
{
    public class IntegrationTestWebAppFactory
    {

    }
}using BasketballAnalytics.Api;
using BasketballAnalytics.Persistence.DbContext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace BasketballAnalytics.Application.IntegrationTests;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
        .WithDatabase("testdb")
        .WithUsername("testuser")
        .WithPassword("testpass")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Αφαιρούμε την παλιά ρύθμιση του DbContext
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            // Προσθέτουμε το DbContext να δείχνει στο container μας
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(_dbContainer.GetConnectionString()));
        });
    }

    // Αυτές οι μέθοδοι του IAsyncLifetime ξεκινούν και σταματούν το container
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}
