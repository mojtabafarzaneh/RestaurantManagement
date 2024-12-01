using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Data;
using RestaurantManagement.Models;

namespace RestaurantManagement.MessageBroker;

public class TicketConsumer
{
    private readonly IModel _channel;
    private readonly ApplicationDBContex _context;
    private readonly ILogger<TicketConsumer> _logger;
    private readonly string _queueName;
    
    
    public TicketConsumer(IConfiguration configuration, 
        IModel channel, 
        ApplicationDBContex context, 
        ILogger<TicketConsumer> logger)
    {
        _channel = channel;
        _context = context;
        _logger = logger;
        _queueName = configuration["RabbitMQ:QueueName"] ?? "ticketQueue";
        
        _channel.QueueDeclare(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    public void StartConsuming()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            await Task.Delay(TimeSpan.FromMinutes(5));
            
            
            _channel.BasicAck(ea.DeliveryTag, false);

            while (!await IsTicketDelayed(message))
            {
                _logger.LogInformation("the ticket is not delayed yet");
                await Task.Delay(TimeSpan.FromMinutes(5));
            }

            await HandleTicketAfterConditionSatisfied(message);
        };
        _channel.BasicConsume(queue: "ticketQueue", autoAck: false, consumer: consumer);
    }

    private async Task<bool> IsTicketDelayed(string message)
    {
        var orders = await _context.Orders
            .Include(x => x.Ticket)
            .ToListAsync();
        if (!orders.Any())
        {
            return false;
        }

        var secondTime = DateTime.Now;
        foreach (var order in orders)
        {
            var orderItems = await _context.OrderItems
                .Include(x => x.Menus)
                .GroupBy(x => x.OrderId)
                .Select(x => new
                {
                    estimatedPrepTime = x.Sum(oi => oi.Quantity * oi.Menus.EstimatedPrepTime)
                })
                .ToListAsync();
            if (orderItems.Count == 0)
            {
                return false;
            }

            foreach (var orderItem in orderItems)
            {
                var firstTime = order.Ticket.CreatedOn.AddMinutes(orderItem.estimatedPrepTime);
                if (secondTime > firstTime) continue;
                order.Ticket.IsFlagged = true;
                order.Ticket.TicketStatus = Ticket.Status.Delayed;
                await _context.SaveChangesAsync();
                return true;
            }
        }
        return false;
    }
    
    private async Task HandleTicketAfterConditionSatisfied(string message)
    {
        Console.WriteLine($"Ticket is delayed. Processing ticket with ID: {message}");
    }
    
}
