using MediatR;
using BasketballAnalytics.Application.Common.Models;
using BasketballAnalytics.Application.Features.Players.Dtos;

namespace BasketballAnalytics.Application.Features.Players.Queries;

public record GetAllPlayersQuery(int Page = 1, int PageSize = 20) : IRequest<PagedResult<PlayerDto>>;
