using MediatR;
using BasketballAnalytics.Application.Features.Players.Dtos;

namespace BasketballAnalytics.Application.Features.Players.Queries;

public record GetPlayersByTeamQuery(Guid TeamId) : IRequest<List<PlayerDto>>;
