using MediatR;
using Microsoft.EntityFrameworkCore;
using BasketballAnalytics.Application.Common.Interfaces;
using BasketballAnalytics.Application.Common.Exceptions;

namespace BasketballAnalytics.Application.Features.Teams.Commands;

public class DeleteTeamCommandHandler : IRequestHandler<DeleteTeamCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteTeamCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteTeamCommand request, CancellationToken cancellationToken)
    {
        var team = await _context.Teams
            // We need to IGNORE the query filter to find a "deleted" team if needed for restore
            // .IgnoreQueryFilters() 
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (team == null)
        {
            throw new NotFoundException(nameof(team), request.Id);
        }

        // Now, we just call Remove(). The DbContext override will handle the rest!
        _context.Teams.Remove(team);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}
