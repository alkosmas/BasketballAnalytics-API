using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BasketballAnalytics.Application.Common.Interfaces;
using MediatR;

namespace BasketballAnalytics.Application.Features.Teams.Commands;

public class UpdateTeamCommandHandler : IRequestHandler<UpdateTeamCommand>
{
    private readonly IApplicationDbContext _context;
    
    public UpdateTeamCommandHandler(IApplicationDbContext context){
        _context = context;
    }

    public async Task Handle(UpdateTeamCommand request, CancellationToken cancellationToken)
    {
        var team = await _context.Teams.FindAsync(new object[]{ request.Id} , cancellationToken);
        if (team == null)
        {
            throw new DllNotFoundException(nameof(team));
        }
        team.Name = request.Name;
        team.City = request.City;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
