using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using BasketballAnalytics.Application.Common.Interfaces;
using BasketballAnalytics.Application.Features.Teams.Dtos;
using BasketballAnalytics.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace BasketballAnalytics.Application.Features.Teams.Commands

{
    public class CreateTeamCommandHandler : IRequestHandler <CreateTeamCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        private readonly IMemoryCache _cache;

        private static readonly string CacheKey = "AllTeamsList";

        public CreateTeamCommandHandler(IApplicationDbContext context, IMemoryCache cache )
        {
            _context = context;
            _cache = cache;
        }
        public async Task<Guid> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
        {
            var team = new Team
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                City = request.City
            };
            _context.Teams.Add(team);

            await _context.SaveChangesAsync(cancellationToken);

            _cache.Remove(CacheKey);


            return team.Id;

        }
        
    }
}