using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Contracts.Responses.OrderResponse;
using RestaurantManagement.Data;
using RestaurantManagement.MessageBroker;
using RestaurantManagement.Models;
using RestaurantManagement.Helper;

namespace RestaurantManagement.Repository;

public class OrderManager:IOrderManager
{
    private readonly ApplicationDBContex _context;
    private readonly RoleHelper _roleHelper;
    private readonly UserHelper _userHelper;
    private readonly TicketProducer _ticketProducer;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderManager> _logger;

    public OrderManager(ApplicationDBContex context, RoleHelper roleHelper, UserHelper userHelper, IMapper mapper, TicketProducer ticketProducer, ILogger<OrderManager> logger)
    {
        _context = context;
        _roleHelper = roleHelper;
        _userHelper = userHelper;
        _mapper = mapper;
        _ticketProducer = ticketProducer;
        _logger = logger;
    }

    public async Task<Card> DoesUserHasCard(Guid id)
    {
        var card = await _context.Cards.FirstOrDefaultAsync(x=> x.CustomerId == id);
        if (card == null)
        {
            throw new UnauthorizedAccessException("you can not create an order for this user.");
        }

        return card;
    }

    public async Task<bool> DoesUserHasOrdered(Guid id)
    {
        var doesOrdered = await _context.Orders.Where(x => x.CustomerId == id).AnyAsync();
        return doesOrdered;
    }

    public async Task CreateOrder(Order request, Guid id, Card card, bool shouldMakeTicket)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try {
                var order = await _context.Orders.AddAsync(request);
                await _context.SaveChangesAsync();
                if (shouldMakeTicket)
                { 
                    var ticket = new Ticket 
                    {
                        OrderId = order.Entity.Id,
                        TicketStatus = Ticket.Status.Waiting
                    };
                    await _context.Tickets.AddAsync(ticket); 
                    await _context.SaveChangesAsync();
                
                    await _ticketProducer.PublishTicketAsync(ticket);
                }

                var cardItems = await _context.CardItems
                    .Include(cardItem => cardItem.Menu)
                    .Where(x => x.CartId == card.Id)
                    .ToListAsync();
            
                if (cardItems == null)
                {
                    throw new Exception("there are no cardItem for this order!");
                }

                foreach (var cardItem in cardItems)
                {
                    if (await _context.OrderItems.AnyAsync(x => x.OrderId == cardItem.MenuId))
                    {
                        continue;
                    }
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Entity.Id,
                        MenuId = cardItem.MenuId,
                        Quantity = cardItem.Quantity,
                        Price = cardItem.Menu.Price,
                    };
                    if (await _context.OrderItems
                            .AnyAsync(x => x.OrderId == order.Entity.Id && x.MenuId == cardItem.MenuId))
                    {
                        throw new Exception("can not make multiple orders for one menuItem");
                    }
                    await _context.OrderItems.AddAsync(orderItem);
                
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<OrderResponse>> GetOrders()
    {
        try
        {
            var orders = await _context.Orders
                .ToListAsync();


            if (orders == null || orders.Count == 0)
            {
                throw new Exception("there are no orders!");
            }

            var ordersMapped = _mapper.Map<List<OrderResponse>>(orders);
            var orderData = await _context.OrderItems
                .Include(oi => oi.Menus)
                .GroupBy(oi => oi.OrderId)
                .Select(g => new
                {
                    estimatedPrepTime = g.Sum(oi => oi.Quantity * oi.Menus.EstimatedPrepTime),
                    totalPrice = g.Sum(oi => oi.Quantity * oi.Price)
                }).ToListAsync();
            for (int i = 0; i < ordersMapped.Count(); i++)
            {
                ordersMapped[i].TotalPrice = orderData[i].totalPrice;
                ordersMapped[i].EstimatedPrepTime = orderData[i].estimatedPrepTime;
            }

            return ordersMapped;
        }
        catch (Exception)
        {
            _logger.LogError("There was an error getting the orders!");
            throw;
        }
    

}

    public async Task<OrderResponse> GetOrderById()
    {
        try
        {
            var userId = _userHelper.UserId();

            if (userId == null)
            {
                throw new UnauthorizedAccessException();
            }

            if (!Guid.TryParse(userId, out var userGuid))
            {
                throw new ArgumentException("Invalid UserId format.");
            }

            var order = await _context.Orders.FirstOrDefaultAsync(x => x.CustomerId == userGuid);

            if (order == null)
            {
                throw new Exception("there are no orders!");
            }

            var mappedOrder = _mapper.Map<OrderResponse>(order);
            var orderData = await _context.OrderItems
                .Where(oi => oi.OrderId == order.Id)
                .Include(oi => oi.Menus)
                .GroupBy(oi => oi.OrderId)
                .Select(g => new
                {
                    TotalPrice = g.Sum(oi => oi.Quantity * oi.Price),
                    EstimatedPrepTime = g.Sum(oi => oi.Quantity * oi.Menus.EstimatedPrepTime),
                }).FirstOrDefaultAsync();
            if (orderData == null)
            {
                throw new KeyNotFoundException("there is no order with this id!");
            }

            mappedOrder.TotalPrice = orderData.TotalPrice;
            mappedOrder.EstimatedPrepTime = orderData.EstimatedPrepTime;

            return mappedOrder;

        }
        catch (Exception)
        {
            _logger.LogError("There was an error getting the order!");
            throw;
        }
        
    }

    public async Task UpdateOrder(Order request)
    {
        try
        {
            var userId = _userHelper.UserId();

            if (!Guid.TryParse(userId, out var userGuid))
            {
                throw new ArgumentException("Invalid UserId format.");
            }


            var order = await _context.Orders
                .Where(x => x.CustomerId == userGuid)
                .FirstOrDefaultAsync(x => x.Id == request.Id);
            if (order == null)
            {
                throw new KeyNotFoundException("there is no order with this id!");
            }

            ArgumentNullException.ThrowIfNull(request);
            request.Id = order.Id;

            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.OrderId == order.Id);

            if (ticket != null)
            {
                switch (request.StatusType)
                {
                    case Order.Status.Completed:
                    case Order.Status.Delivered:
                        ticket.TicketStatus = Ticket.Status.Served;
                        break;
                    case Order.Status.Cancelled:
                        _context.Tickets.Remove(ticket);
                        break;
                    case Order.Status.Delayed:
                        ticket.TicketStatus = Ticket.Status.Delayed;
                        ticket.IsFlagged = true;
                        break;
                }

            }

            order.StatusType = request.StatusType;
            await _context.SaveChangesAsync();
        }

        catch (Exception)
        {
            _logger.LogError("There was an error updating the order!");
            throw;
        }
            
    }

    public async Task DeleteOrder()
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var userId = _userHelper.UserId();
            var isChef = _roleHelper.IsUserChef();
            var isManager = _roleHelper.IsManagerUser();

            if (userId == null)
            {
                throw new UnauthorizedAccessException();
            }

            if (!Guid.TryParse(userId, out var userGuid))
            {
                throw new ArgumentException("Invalid UserId format.");
            }

            var isOrder = await _context.Orders.Where(x => x.CustomerId == userGuid).FirstOrDefaultAsync();
            if (isOrder == null && !isChef && !isManager)
            {
                throw new UnauthorizedAccessException("you can not get other users orders.");
            }
            
            var orderItems = await _context.OrderItems
                .Where(oi => isOrder != null && oi.OrderId == isOrder.Id).ToListAsync();
            if (orderItems == null || orderItems.Count == 0)
            {
                throw new KeyNotFoundException("there is no order with this id!");
            }

            foreach (var i in orderItems)
            {
                _context.OrderItems.Remove(i);
            }

            await _context.SaveChangesAsync();

            var ticket = await _context.Tickets.FirstOrDefaultAsync(x => isOrder != null && x.OrderId == isOrder.Id) ;
            
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
                
            }

            if (isOrder != null) _context.Orders.Remove(isOrder);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
        
    }

    public async Task<Order>IsExist(Guid id)
    {
        try
        {
            var entity = await _context.Orders.FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("there is no order with this id!");
            }

            return entity;
        }
        catch (Exception)
        {
            _logger.LogError("There was an error getting the order!");
            throw;
        }
        
    }

    public async Task<bool> DoesCustomerExist(Guid id)
    {
        var entity = await _context.Customers.FindAsync(id);
        return entity != null;
    }

    public async Task<Ticket> GetTicketById()
    {
        try
        {
            var userId = _userHelper.UserId();

            if (userId == null)
            {
                throw new UnauthorizedAccessException();
            }

            if (!Guid.TryParse(userId, out var userGuid))
            {
                throw new ArgumentException("Invalid UserId format.");
            }

            var orders = await _context.Orders
                .Include(x => x.Ticket)
                .Where(x => x.CustomerId == userGuid)
                .FirstOrDefaultAsync();

            if (orders == null)
            {
                throw new KeyNotFoundException("there is no order with this id!");
            }

            var orderItems = await _context.OrderItems
                .Include(oi => oi.Menus)
                .Where(oi => oi.OrderId == orders.Id)
                .GroupBy(oi => oi.OrderId)
                .Select(g => new
                {
                    estimatedPrepTime = g.Sum(oi => oi.Quantity * oi.Menus.EstimatedPrepTime)
                })
                .ToListAsync();
            if (orderItems == null || orderItems.Count == 0)
            {
                throw new KeyNotFoundException("there is no order with this id!");
            }

            if (orders.Ticket == null)
            {
                throw new KeyNotFoundException("there is no order Ticket with this id!");
            }

            var secondTime = DateTime.Now;

            foreach (var orderItem in orderItems)
            {
                var firstTime = orders.Ticket.CreatedOn.AddMinutes(orderItem.estimatedPrepTime);

                if (secondTime < firstTime) continue;
                orders.Ticket.IsFlagged = true;
                orders.Ticket.TicketStatus = Ticket.Status.Delayed;

            }

            await _context.SaveChangesAsync();
            return orders.Ticket;
        }
        catch (Exception)
        {
            _logger.LogError("There was an error getting the order!");
            throw;
        }
        
        
    }
}