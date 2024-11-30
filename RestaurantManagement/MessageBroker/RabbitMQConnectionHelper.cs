using RabbitMQ.Client;

namespace RestaurantManagement.MessageBroker;

public class RabbitMQConnectionHelper
{
    private readonly IConfiguration _configuration;

    public RabbitMQConnectionHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IConnection GetConnection()
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"],
            UserName = _configuration["RabbitMQ:UserName"],
            Password = _configuration["RabbitMQ:Password"],
        };
        return factory.CreateConnection();
    }
}