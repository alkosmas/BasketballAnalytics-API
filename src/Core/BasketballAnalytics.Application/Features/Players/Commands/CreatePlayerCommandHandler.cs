using MediatR;
using BasketballAnalytics.Application.Common.Interfaces;
using BasketballAnalytics.Domain.Entities;

namespace BasketballAnalytics.Application.Features.Players.Commands;

public class CreatePlayerCommandHandler : IRequestHandler<CreatePlayerCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreatePlayerCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreatePlayerCommand request, CancellationToken cancellationToken)
    {
        var player = new Player
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            HeightCm = request.HeightCm,
            WeightKg = request.WeightKg,
            Position = (Position)request.Position,
            JerseyNumber = request.JerseyNumber,
            TeamId = request.TeamId
        };

        _context.Players.Add(player);
        await _context.SaveChangesAsync(cancellationToken);

        return player.Id;
    }
}
