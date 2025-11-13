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
    public class GetTeamByIdQueryHandler : IRequestHandler <GetTeamByIdQuery, TeamDto?>
    {
        private readonly IApplicationDbContext  _context;

        public GetTeamByIdQueryHandler (IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<TeamDto?> Handle(GetTeamByIdQuery request , CancellationToken cancellationToken)
        {
            var team = await _context.Teams
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);
        
            if(team is null){
                return null;
            }
            return new TeamDto
            {
                Id = team.Id,
                Name = team.Name,
                City = team.City,
            };   
        }
    }
}