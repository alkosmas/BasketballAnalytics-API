using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BasketballAnalytics.Application.Features.Teams.Commands;
using BasketballAnalytics.Application.Features.Teams.Dtos;
using FluentAssertions;
using Xunit;

namespace BasketballAnalytics.Tests.Common;



public class TeamsControllerTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;

    public TeamsControllerTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateTeam_WithValidData_ShouldReturnCreatedAndLocationHeader()
    {
        var createCommand = new CreateTeamCommand("Los Angeles Lakers", "Los Angeles");


        var response = await _client.PostAsJsonAsync("/api/teams", createCommand);

     
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var locationHeader = response.Headers.Location;
        locationHeader.Should().NotBeNull();

        var getResponse = await _client.GetAsync(locationHeader);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var createdTeam = await getResponse.Content.ReadFromJsonAsync<TeamDto>();
        createdTeam.Should().NotBeNull();
        createdTeam.Name.Should().Be(createCommand.Name);
        createdTeam.City.Should().Be(createCommand.City);
    }

    [Fact]
    public async Task CreateTeam_WithInvalidData_ShouldReturnBadRequest()
    {

        var createCommand = new CreateTeamCommand("", "Invalid City");

        var response = await _client.PostAsJsonAsync("/api/teams", createCommand);


        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

