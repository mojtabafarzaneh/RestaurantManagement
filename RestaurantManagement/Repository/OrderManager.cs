using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Contracts.Responses.OrderResponse;
using RestaurantManagement.Data;
using RestaurantManagement.Models;
using RestaurantManagement.Services;

namespace RestaurantManagement.Repository;

public class OrderManager:IOrderManager
{
    private readonly ApplicationDBContex _context;
    private readonly RoleService _roleService;
    private readonly UserService _userService;
    private readonly IMapper _mapper;

    public OrderManager(ApplicationDBContex context, RoleService roleService, UserService userService, IMapper mapper)
    {
        _context = context;
        _roleService = roleService;
        _userService = userService;
        _mapper = mapper;
    }

    public async Task CreateOrder(Order request)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        var userId = _userService.UserId();
        if (userId == null)
        {
            throw new UnauthorizedAccessException();
        }
        if (!Guid.TryParse(userId, out var userGuid))
        {
            throw new ArgumentException("Invalid UserId format.");
        }
        var card = await _context.Cards.FirstOrDefaultAsync(x=> x.CustomerId == userGuid);
        if (card == null)
        {
            throw new UnauthorizedAccessException("you can not create an order for this user.");
        }
        request.CustomerId = userGuid;
        request.StatusType = request.TypeOfOrder switch
        {
            Order.OrderType.Delivery or Order.OrderType.Online => Order.Status.Pending,
            Order.OrderType.InHouse => Order.Status.Preparing,
            _ => request.StatusType
        };
        try
        {
            var order = await _context.Orders.AddAsync(request);
            await _context.SaveChangesAsync();
            if (order.Entity.StatusType == Order.Status.Preparing)
            {
                var ticket = new Ticket
                {
                    OrderId = order.Entity.Id,
                    TicketStatus = Ticket.Status.Waiting
                };
                await _context.Tickets.AddAsync(ticket);
                var ticketChange = await _context.SaveChangesAsync();
            }

            var cardItem = await _context.CardItems.Include(cardItem => cardItem.Menu).FirstOrDefaultAsync(x => x.CartId == card.Id);
            if (cardItem == null)
            {
                throw new Exception("there are no cardItem for this order!");
            }

            var orderItem = new OrderItem
            {
                OrderId = order.Entity.Id,
                MenuId = cardItem.MenuId,
                Quantity = cardItem.Quantity,
                Price = cardItem.Menu.Price,
            };
            await _context.OrderItems.AddAsync(orderItem);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
        }
    }

    public Task<List<Order>> GetOrders()
    {
        throw new NotImplementedException();
    }

    public async Task<OrderResponse> GetOrderById(Guid id)
    {
        //TODO: make chef and manager could visit this endpoint!
        var userId = _userService.UserId();
        
        if (userId == null)
        {
            throw new UnauthorizedAccessException();
        }
        if (!Guid.TryParse(userId, out var userGuid))
        {
            throw new ArgumentException("Invalid UserId format.");
        }

        var order = await _context.Orders
            .Where(o => o.CustomerId == userGuid)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (order == null)
        {
            throw new KeyNotFoundException("there is no order with this id!");
        }
        var mappedOrder = _mapper.Map<OrderResponse>(order);
        
        mappedOrder.TotalPrice = await _context.OrderItems
            .Where(oi => oi.OrderId == id).
            SumAsync(oi => oi.Quantity * oi.Price);
        mappedOrder.EstimatedPrepTime = await _context.OrderItems
            .Where(oi => oi.OrderId == id)
            .Include(oi => oi.Menus)
            .SumAsync(oi => oi.Quantity * oi.Menus.EstimatedPrepTime);
        
        return mappedOrder;

    }

    public Task UpdateOrder(Order order)
    {
        throw new NotImplementedException();
    }

    public Task DeleteOrder(Guid id)
    {
        throw new NotImplementedException();
    }
}