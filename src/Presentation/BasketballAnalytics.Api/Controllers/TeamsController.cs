using BasketballAnalytics.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BasketballAnalytics.Domain.Entities;
using MediatR;
using BasketballAnalytics.Application.Features.Teams.Queries;
using BasketballAnalytics.Application.Features.Teams.Commands;
using BasketballAnalytics.Application.Features.Teams.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace BasketballAnalytics.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TeamsController : ControllerBase
{
    private readonly ISender _mediator;

    public TeamsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
    {
        var teams = await _mediator.Send(new GetAllTeamsQuery());
        return Ok(teams);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Team>> GetTeamById(Guid id)
    {
        var query = new GetTeamByIdQuery(id);
        var team = await _mediator.Send(query);
        return team is not null ? Ok(team) : NotFound();
    }

    [HttpGet("{id:guid}/stats")]
    public async Task<ActionResult<TeamStatsDto>> GetTeamStats(Guid id)
    {
        var stats = await _mediator.Send(new GetTeamStatsQuery(id));
        return stats is not null ? Ok(stats) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult> CreateTeam([FromBody] CreateTeamCommand command)
    {
        var teamId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTeamById), new { id = teamId }, teamId);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateTeam(Guid id, [FromBody] UpdateTeamCommand request)
    {
        var command = new UpdateTeamCommand(id, request.Name, request.City);
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<NoContentResult> DeleteTeam(Guid id)
    {
        var command = new DeleteTeamCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }
}
