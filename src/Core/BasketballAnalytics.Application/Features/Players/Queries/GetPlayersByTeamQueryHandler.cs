using MediatR;
using Microsoft.EntityFrameworkCore;
using BasketballAnalytics.Application.Common.Interfaces;
using BasketballAnalytics.Application.Features.Players.Dtos;

namespace BasketballAnalytics.Application.Features.Players.Queries;

public class GetPlayersByTeamQueryHandler : IRequestHandler<GetPlayersByTeamQuery, List<PlayerDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPlayersByTeamQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PlayerDto>> Handle(GetPlayersByTeamQuery request, CancellationToken cancellationToken)
    {
        var players = await _context.Players
            .AsNoTracking()
            .Where(p => p.TeamId == request.TeamId)
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

        return players;
    }
}
