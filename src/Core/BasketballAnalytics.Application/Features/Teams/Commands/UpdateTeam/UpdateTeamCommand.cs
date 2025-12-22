
using MediatR;

namespace BasketballAnalytics.Application.Features.Teams.Commands;

public record UpdateTeamCommand(Guid Id,  string Name, string City):IRequest;
