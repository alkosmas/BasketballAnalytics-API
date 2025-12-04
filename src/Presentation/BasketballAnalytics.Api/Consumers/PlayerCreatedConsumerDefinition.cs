using MassTransit;

namespace BasketballAnalytics.Api.Consumers;

public class PlayerCreatedConsumerDefinition : ConsumerDefinition<PlayerCreatedConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<PlayerCreatedConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        // Retry policy with exponential backoff
        endpointConfigurator.UseMessageRetry(r =>
        {
            r.Intervals(
                TimeSpan.FromSeconds(5),   // 1st retry: wait 5 seconds
                TimeSpan.FromSeconds(15),  // 2nd retry: wait 15 seconds
                TimeSpan.FromSeconds(30)   // 3rd retry: wait 30 seconds
            );
            
            // Only retry on specific exceptions
            r.Ignore<ArgumentNullException>();  // Don't retry bad data
        });
    }
}
