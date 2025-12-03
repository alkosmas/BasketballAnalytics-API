using MediatR;
using Microsoft.EntityFrameworkCore;
using BasketballAnalytics.Application.Common.Interfaces;
using BasketballAnalytics.Application.Features.Teams.Dtos;

namespace BasketballAnalytics.Application.Features.Teams.Queries;

public class GetTeamStatsQueryHandler : IRequestHandler<GetTeamStatsQuery, TeamStatsDto?>
{
    private readonly IApplicationDbContext _context;

    public GetTeamStatsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TeamStatsDto?> Handle(GetTeamStatsQuery request, CancellationToken cancellationToken)
    {
        var team = await _context.Teams
            .AsNoTracking()
            .Where(t => t.Id == request.TeamId)
            .Select(t => new TeamStatsDto
            {
                TeamId = t.Id,
                TeamName = t.Name,
                City = t.City,
                PlayerCount = t.Players.Count,
                AverageHeightCm = t.Players.Any() 
                    ? Math.Round(t.Players.Average(p => p.HeightCm), 1)
                    : 0,
                AverageWeightKg = t.Players.Any()
                    ? Math.Round(t.Players.Average(p => p.WeightKg), 1)
                    : 0
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (team == null)
            return null;

        // Position breakdown - ξεχωριστό query για grouping
        var positionBreakdown = await _context.Players
            .AsNoTracking()
            .Where(p => p.TeamId == request.TeamId)
            .GroupBy(p => p.Position)
            .Select(g => new { Position = g.Key.ToString(), Count = g.Count() })
            .ToDictionaryAsync(x => x.Position, x => x.Count, cancellationToken);

        team.PositionBreakdown = positionBreakdown;

        return team;
    }
}
