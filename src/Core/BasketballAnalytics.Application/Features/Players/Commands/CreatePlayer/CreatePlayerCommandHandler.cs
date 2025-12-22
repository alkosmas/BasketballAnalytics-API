using MediatR;
using MassTransit;
using BasketballAnalytics.Application.Common.Interfaces;
using BasketballAnalytics.Application.Common.Events;
using BasketballAnalytics.Domain.Entities;

namespace BasketballAnalytics.Application.Features.Players.Commands;

public class CreatePlayerCommandHandler : IRequestHandler<CreatePlayerCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreatePlayerCommandHandler(
        IApplicationDbContext context,
        IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
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

        // Publish event to RabbitMQ (async processing)
        await _publishEndpoint.Publish(new PlayerCreatedEvent
        {
            PlayerId = player.Id,
            FirstName = player.FirstName,
            LastName = player.LastName,
            TeamId = player.TeamId,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        return player.Id;
    }
}
