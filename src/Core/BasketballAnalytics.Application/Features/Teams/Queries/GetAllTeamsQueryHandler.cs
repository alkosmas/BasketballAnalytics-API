using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasketballAnalytics.Application.Features.Teams.Dtos;
using BasketballAnalytics.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using Microsoft.Extensions.Caching.Memory;

namespace BasketballAnalytics.Application.Features.Teams.Queries
{
    public class GetAllTeamsQueryHandler: IRequestHandler <GetAllTeamsQuery , List<TeamDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        private static readonly string CacheKey = "AllTeamsList";

        public GetAllTeamsQueryHandler(IApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }
    
        public async Task<List<TeamDto>> Handle(GetAllTeamsQuery request, CancellationToken cancellationToken)
        {

            if (_cache.TryGetValue(CacheKey, out List<TeamDto> cachedTeams)){
                return cachedTeams;
            }

            var teams = await _context.Teams
                    .AsNoTracking()
                    .OrderBy(t => t.Name)
                    .ToListAsync(cancellationToken);

            var teamDtos = teams.Select(team => new TeamDto
            {
                Id = team.Id,
                Name = team.Name,
                City = team.City
            }).ToList();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

            _cache.Set(CacheKey, teamDtos, cacheEntryOptions);

            return teamDtos;
        }
    }
}