using MediatR;
using Microsoft.EntityFrameworkCore;
using BasketballAnalytics.Application.Common.Interfaces;
using BasketballAnalytics.Application.Common.Models;
using BasketballAnalytics.Application.Features.Players.Dtos;

namespace BasketballAnalytics.Application.Features.Players.Queries;

public class GetAllPlayersQueryHandler : IRequestHandler<GetAllPlayersQuery, PagedResult<PlayerDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllPlayersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<PlayerDto>> Handle(GetAllPlayersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Players.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var players = await query
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new PlayerDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                HeightCm = p.HeightCm,
                WeightKg = p.WeightKg,
                Position = p.Position.ToString(),
                JerseyNumber = p.JerseyNumber,
                TeamId = p.TeamId,
                TeamName = p.Team.Name
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<PlayerDto>
        {
            Items = players,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
