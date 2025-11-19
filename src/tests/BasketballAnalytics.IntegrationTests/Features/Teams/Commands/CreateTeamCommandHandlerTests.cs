using BasketballAnalytics.Application.Features.Teams.Commands;
using BasketballAnalytics.Persistence.DbContext;
using FluentAssertions; // Μια πολύ καλή βιβλιοθήκη για assertions
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BasketballAnalytics.Application.IntegrationTests.Features.Teams.Commands;

public class CreateTeamCommandHandlerTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly IServiceScope _scope;
    private readonly ISender _mediator;
    private readonly ApplicationDbContext _dbContext;

    public CreateTeamCommandHandlerTests(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        _mediator = _scope.ServiceProvider.GetRequiredService<ISender>();
        _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    [Fact]
    public async Task Handle_Should_CreateTeam_And_PersistInDatabase()
    {
        // --- Arrange ---
        // 1. Εφάρμοσε τα migrations στην test database.
        await _dbContext.Database.MigrateAsync();

        // 2. Δημιούργησε το command
        var command = new CreateTeamCommand("Chicago Bulls", "Chicago");

        // --- Act ---
        // 3. Εκτέλεσε το command μέσω του MediatR
        var teamId = await _mediator.Send(command);

        // --- Assert ---
        // 4. Έλεγξε απευθείας στη βάση δεδομένων
        var createdTeam = await _dbContext.Teams.FindAsync(teamId);

        // 5. Χρησιμοποίησε το FluentAssertions για πιο "καθαρές" επιβεβαιώσεις
        createdTeam.Should().NotBeNull();
        createdTeam.Name.Should().Be(command.Name);
        createdTeam.City.Should().Be(command.City);
    }
}