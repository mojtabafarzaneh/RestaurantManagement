using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using RabbitMQ.Client;
using RestaurantManagement.Models;

namespace RestaurantManagement.MessageBroker;

public class TicketProducer
{
    private readonly ILogger<TicketProducer> _logger;
    private readonly RabbitMQConnectionHelper _connectionHelper;
    private readonly IConfiguration _configuration;
    public TicketProducer(RabbitMQConnectionHelper connectionHelper, IConfiguration configuration, ILogger<TicketProducer> logger)
    {
        _connectionHelper = connectionHelper;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task PublishTicketAsync(Ticket ticket)
    {
        var connection = _connectionHelper.GetConnection();
        using var channel = connection.CreateModel();

        var exchange = _configuration["RabbitMQ:Exchange"];
        var routingKey = _configuration["RabbitMQ:RoutingKey"];

        channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Direct);

        var options = new JsonSerializerOptions
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
            WriteIndented = true
        };

        var message = JsonSerializer.Serialize(ticket, options);
        var body = Encoding.UTF8.GetBytes(message);

        await Task.Run(() => channel.BasicPublish(exchange: exchange, routingKey: routingKey, basicProperties: null, body: body));

        _logger.LogInformation($"Message published to {exchange}");
    }

}