using MassTransit;
using BasketballAnalytics.Application.Common.Events;

namespace BasketballAnalytics.Api.Consumers;

public class PlayerCreatedConsumer : IConsumer<PlayerCreatedEvent>
{
    private readonly ILogger<PlayerCreatedConsumer> _logger;

    public PlayerCreatedConsumer(ILogger<PlayerCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PlayerCreatedEvent> context)
    {
        var message = context.Message;
        
        _logger.LogInformation(
            "Processing PlayerCreatedEvent: {FirstName} {LastName} (ID: {PlayerId})",
            message.FirstName,
            message.LastName,
            message.PlayerId);

        // Simulate work (email, notifications, etc.)
          await SimulateSendWelcomeEmail(message);
        await SimulateUpdateTeamStats(message);
        await SimulateNotifyCoach(message);


        _logger.LogInformation(
            "Completed processing for player {PlayerId}. Would send welcome email, update stats, notify coach.",
            message.PlayerId);
    }

    private async Task SimulateSendWelcomeEmail(PlayerCreatedEvent message)
    {
        // Simulate occasional failure (for testing retry)
        // Remove this in production!
        if (Random.Shared.Next(100) < 30) // 30% chance to fail
        {
            _logger.LogWarning("Email service temporarily unavailable!");
            throw new Exception("Email service is down");
        }

        await Task.Delay(100);
        _logger.LogInformation("Welcome email sent to player {PlayerId}", message.PlayerId);
    }
    
    private async Task SimulateUpdateTeamStats(PlayerCreatedEvent message)
    {
        await Task.Delay(50);
        _logger.LogInformation("Team stats updated for team {TeamId}", message.TeamId);
    }

    private async Task SimulateNotifyCoach(PlayerCreatedEvent message)
    {
        await Task.Delay(50);
        _logger.LogInformation("Coach notified about new player {PlayerId}", message.PlayerId);
    }
}
