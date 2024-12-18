﻿using System.Net;
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

    public async Task<List<Order>> DoesAnyOrdersExist()
    {
        var orders = await _context.Orders
            .ToListAsync();
        
        return orders;
        
    }

    public async Task<List<OrderResponse>> GetOrders(List<Order> orders)
    {
        try
        {

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
    
    public async Task<Order?> FindOrderById(Guid id)
    {
        var order =await _context.Orders.FirstOrDefaultAsync(x => x.CustomerId == id);
        return order ?? null;
    }

    public async Task<OrderResponse> GetOrderById(Order order)
    {
        try
        {
            
            var mappedOrder = _mapper.Map<OrderResponse>(order);
            var orderData = await _context.OrderItems
                .Where(oi => oi.OrderId == order!.Id)
                .Include(oi => oi.Menus)
                .GroupBy(oi => oi.OrderId)
                .Select(g => new
                {
                    TotalPrice = g.Sum(oi => oi.Quantity * oi.Price),
                    EstimatedPrepTime = g.Sum(oi => oi.Quantity * oi.Menus.EstimatedPrepTime),
                }).FirstOrDefaultAsync();
            if (orderData == null)
            {
                throw new KeyNotFoundException("there is no orderItem with this id!");
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

    public async Task RemoveTicket(Ticket ticket)
    {
        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync();
    }

    public async Task<Ticket?> FindTicket(Guid id)
    {
        
        var ticket = await _context.Tickets
            .FirstOrDefaultAsync(t => t.OrderId == id);
        return ticket ?? null;
    }

    public async Task UpdateOrder(Order request)
    {

        ArgumentNullException.ThrowIfNull(request);
        await _context.SaveChangesAsync();

        
    }

    public async Task<Order?> GetOrderById(Guid id)
    {
        var isOrder = await _context.Orders.Where(x => x.CustomerId == id).FirstOrDefaultAsync();

        if (isOrder == null)
        {
            return null;
        }
        return isOrder;

    }

    public async Task DeleteOrder(Order order)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            
            var orderItems = await _context.OrderItems
                .Where(oi => oi.OrderId == order.Id).ToListAsync();
            if (orderItems == null || orderItems.Count == 0)
            {
                throw new KeyNotFoundException("there is no order with this id!");
            }

            foreach (var i in orderItems)
            {
                _context.OrderItems.Remove(i);
            }

            await _context.SaveChangesAsync();

            var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.OrderId == order.Id) ;
            
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
                
            }

            _context.Orders.Remove(order);
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
    

    public async Task<Ticket> GetTicketById(Guid userGuid)
    {
        try
        {
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