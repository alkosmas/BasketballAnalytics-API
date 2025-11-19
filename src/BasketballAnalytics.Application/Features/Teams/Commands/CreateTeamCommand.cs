using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace BasketballAnalytics.Application.Features.Teams.Commands;


public record CreateTeamCommand(string Name, string City) : IRequest<Guid>;
