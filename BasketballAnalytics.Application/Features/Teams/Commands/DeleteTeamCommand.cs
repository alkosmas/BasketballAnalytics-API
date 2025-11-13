using MediatR;

namespace BasketballAnalytics.Application.Features.Teams.Commands;

public record DeleteTeamCommand(Guid Id) : IRequest;