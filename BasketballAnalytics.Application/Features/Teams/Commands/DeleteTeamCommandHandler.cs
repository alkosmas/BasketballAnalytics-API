using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasketballAnalytics.Application.Common.Interfaces;
using MediatR;

namespace BasketballAnalytics.Application.Features.Teams.Commands
{
public class DeleteTeamCommandHandler : IRequestHandler<DeleteTeamCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteTeamCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteTeamCommand request, CancellationToken cancellationToken)
    {
        var team = await _context.Teams.FindAsync(new object[] { request.Id }, cancellationToken);

        if (team is null)
        {
            throw new KeyNotFoundException(request.Id.ToString());
        }

        _context.Teams.Remove(team);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
}