using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasketballAnalytics.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BasketballAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MediatR;
using BasketballAnalytics.Application.Features.Teams.Queries;
using BasketballAnalytics.Application.Features.Teams.Commands;

namespace BasketballAnalytics.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            var teams =  await _mediator.Send(new GetAllTeamsQuery());
            return Ok(teams);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Team>> GetTeamById(Guid id)
        {
            var query = new GetTeamByIdQuery(id);
            var team = await _mediator.Send(query);
            return team is not null ? Ok(team) : NotFound();
        }
        
        [HttpPost]
        public async Task<ActionResult> CreateTeam([FromBody] CreateTeamCommand command)
        {
            var teamId = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetTeamById), new { id = teamId} , teamId);
        }

          
        [HttpPut("{id:guid}")]
        public async Task<ActionResult> UpdateTeam(Guid id,[FromBody] UpdateTeamCommand request)
        {
            var command = new UpdateTeamCommand(id , request.Name, request.City);

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<NoContentResult> DeleteTeam(Guid id)
        {
            var command = new DeleteTeamCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }
    }


}