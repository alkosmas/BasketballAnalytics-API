using BasketballAnalytics.Application.Features.Teams.Dtos;
using MediatR;

namespace BasketballAnalytics.Application.Features.Teams.Queries;

public record GetAllTeamsQuery : IRequest<List<TeamDto>>;