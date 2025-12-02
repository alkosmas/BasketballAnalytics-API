using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasketballAnalytics.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace BasketballAnalytics.Application.Features.Teams.Commands
{
public class DeleteTeamCommandHandler : IRequestHandler<DeleteTeamCommand>
{
    private readonly IApplicationDbContext _context;

    private readonly IMemoryCache _cache;

    private static readonly string CacheKey = "AllTeamsList";

    public DeleteTeamCommandHandler(IApplicationDbContext context , IMemoryCache cache)
    {
         _context = context;
         _cache = cache;
    }

    public async Task Handle(DeleteTeamCommand request, CancellationToken cancellationToken)
    {
        var team = await _context.Teams.FindAsync(new object[] { request.Id }, cancellationToken);

        if (team is null)
        {
            throw new KeyNotFoundException(request.Id.ToString());
        }

        _context.Teams.Remove(team);

        await _context.SaveChangesAsync(cancellationToken);
               
        _cache.Remove(CacheKey);


    }
 }
}