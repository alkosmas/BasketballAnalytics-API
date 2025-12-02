using BasketballAnalytics.Application.Features.Teams.Dtos;
using MediatR;

namespace BasketballAnalytics.Application.Features.Teams.Queries;

public record GetTeamByIdQuery(Guid Id) : IRequest<TeamDto?>;
