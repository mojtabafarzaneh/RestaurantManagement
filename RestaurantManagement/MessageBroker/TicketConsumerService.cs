namespace RestaurantManagement.MessageBroker;

public class TicketConsumerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public TicketConsumerService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var consumer = scope.ServiceProvider.GetRequiredService<TicketConsumer>();
        consumer.StartConsuming();

        // Prevent the service from exiting
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
