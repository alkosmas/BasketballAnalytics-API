using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasketballAnalytics.Application.Features.Teams.Dtos;
using BasketballAnalytics.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace BasketballAnalytics.Application.Features.Teams.Queries
{
    public class GetAllTeamsQueryHandler: IRequestHandler <GetAllTeamsQuery , List<TeamDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllTeamsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
    
        public async Task<List<TeamDto>> Handle(GetAllTeamsQuery request, CancellationToken cancellationToken)
        {
        var teams = await _context.Teams.AsNoTracking().ToListAsync(cancellationToken);
        var teamDtos = teams.Select(team => new TeamDto
        {
            Id = team.Id,
            Name = team.Name,
            City = team.City
        }).ToList();

        return teamDtos;
        }
    }
}