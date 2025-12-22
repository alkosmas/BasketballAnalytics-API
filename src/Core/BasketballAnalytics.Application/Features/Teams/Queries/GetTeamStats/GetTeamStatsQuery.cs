using MediatR;
using BasketballAnalytics.Application.Features.Teams.Dtos;

namespace BasketballAnalytics.Application.Features.Teams.Queries;

public record GetTeamStatsQuery(Guid TeamId) : IRequest<TeamStatsDto?>;
