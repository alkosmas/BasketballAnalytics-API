using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BasketballAnalytics.Application.Features.Players.Commands;
using BasketballAnalytics.Application.Features.Players.Queries;
using BasketballAnalytics.Application.Features.Players.Dtos;

namespace BasketballAnalytics.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PlayersController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlayersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("team/{teamId:guid}")]
    public async Task<ActionResult<List<PlayerDto>>> GetByTeam(Guid teamId)
    {
        var players = await _mediator.Send(new GetPlayersByTeamQuery(teamId));
        return Ok(players);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreatePlayerCommand command)
    {
        var playerId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetByTeam), new { teamId = command.TeamId }, playerId);
    }
}
