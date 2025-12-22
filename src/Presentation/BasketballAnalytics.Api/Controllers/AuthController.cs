using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasketballAnalytics.Application.Features.Teams.Commands;
using BasketballAnalytics.Application.Features.Teams.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BasketballAnalytics.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AuthController : ControllerBase
    {
        private readonly ISender _mediator;

        public  AuthController(ISender mediator)
        {
                _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register (RegisterUserCommand command)
        {
            var userId = await _mediator.Send(command);
            return Ok(new { userId = userId });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginQuery query)
        {
            var token = await _mediator.Send(query);
            return Ok(new { token = token });   
        }
    }
}