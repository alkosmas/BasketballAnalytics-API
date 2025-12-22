using MediatR;

namespace BasketballAnalytics.Application.Features.Players.Commands;

public record CreatePlayerCommand(
    string FirstName,
    string LastName,
    int HeightCm,
    int WeightKg,
    int Position,
    string JerseyNumber,
    Guid TeamId
) : IRequest<Guid>;
