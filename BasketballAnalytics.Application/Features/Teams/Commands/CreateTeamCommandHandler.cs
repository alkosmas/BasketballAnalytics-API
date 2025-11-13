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

namespace BasketballAnalytics.Application.Features.Teams.Commands

{
    public class CreateTeamCommandHandler : IRequestHandler <CreateTeamCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public CreateTeamCommandHandler(IApplicationDbContext context)
        {
            _context = context;
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
            return team.Id;

        }
        
    }
}