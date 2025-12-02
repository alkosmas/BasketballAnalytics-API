using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BasketballAnalytics.Application.Features.Teams.Commands;
using BasketballAnalytics.Application.Features.Teams.Dtos;
using FluentAssertions;
using Xunit;

namespace BasketballAnalytics.Tests.Common;


// Χρησιμοποιούμε το IClassFixture για να δημιουργηθεί μία φορά η factory για όλα τα tests.
// Ας υποθέσουμε ότι η IntegrationTestWebAppFactory είναι προσβάσιμη εδώ.
public class TeamsControllerTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;

    public TeamsControllerTests(IntegrationTestWebAppFactory factory)
    {
        // Η factory μας δίνει έναν έτοιμο HttpClient που "μιλάει" με την in-memory εφαρμογή μας.
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateTeam_WithValidData_ShouldReturnCreatedAndLocationHeader()
    {
        // --- Arrange ---
        // 1. Δημιούργησε τα δεδομένα που θα στείλεις
        var createCommand = new CreateTeamCommand("Los Angeles Lakers", "Los Angeles");

        // --- Act ---
        // 2. Στείλε το POST request στο endpoint
        var response = await _client.PostAsJsonAsync("/api/teams", createCommand);

        // --- Assert ---
        // 3. Έλεγξε το Status Code
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // 4. Έλεγξε ότι το Location header δεν είναι κενό
        var locationHeader = response.Headers.Location;
        locationHeader.Should().NotBeNull();

        // 5. (Προχωρημένο Assert) Κάνε ένα GET request στο νέο URL για να επιβεβαιώσεις τη δημιουργία
        var getResponse = await _client.GetAsync(locationHeader);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // 6. Διάβασε το JSON της απάντησης και έλεγξε τα δεδομένα
        var createdTeam = await getResponse.Content.ReadFromJsonAsync<TeamDto>();
        createdTeam.Should().NotBeNull();
        createdTeam.Name.Should().Be(createCommand.Name);
        createdTeam.City.Should().Be(createCommand.City);
    }

    [Fact]
    public async Task CreateTeam_WithInvalidData_ShouldReturnBadRequest()
    {
        // --- Arrange ---
        // Στέλνουμε κενό όνομα για να αποτύχει το FluentValidation
        var createCommand = new CreateTeamCommand("", "Invalid City");

        // --- Act ---
        var response = await _client.PostAsJsonAsync("/api/teams", createCommand);

        // --- Assert ---
        // Ελέγχουμε ότι το API απάντησε σωστά με 400 Bad Request
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}