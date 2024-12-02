﻿using RestaurantManagement.Contracts.Responses.OrderResponse;
using RestaurantManagement.Helper;
using RestaurantManagement.Models;
using RestaurantManagement.Repository;

namespace RestaurantManagement.Services;

public class OrderService: IOrderService
{
    private readonly IOrderManager _orderManager;
    private readonly ILogger<OrderService> _logger;
    private readonly RoleHelper _roleHelper;
    private readonly UserHelper _userHelper;

    public OrderService(IOrderManager orderManager,
        ILogger<OrderService> logger,
        RoleHelper roleHelper, UserHelper userHelper)
    {
        _orderManager = orderManager;
        _logger = logger;
        _roleHelper = roleHelper;
        _userHelper = userHelper;
    }

    public async Task CreateOrder(Order request)
    {
        try
        {
            bool shouldMakeTicket = false; 
            var userId = _userHelper.UserId();
            if (userId == null)
            {
                throw new UnauthorizedAccessException();
            }
            if (!Guid.TryParse(userId, out var userGuid))
            {
                throw new ArgumentException("Invalid UserId format.");
            }
            if (request.TypeOfOrder == 0)
            {
                throw new ArgumentException("Type of order cannot be empty");
            }
            if (request.TableNumber is > 10 or < 0 ||
                request is { TypeOfOrder: Order.OrderType.InHouse, TableNumber: null })
            {
                throw new ArgumentException("Table number must be between 0 and 10");
            }

            if (request is { TypeOfOrder: Order.OrderType.Delivery, TableNumber: not null } ||
                request is { TypeOfOrder: Order.OrderType.Online, TableNumber: not null })
            {
                throw new ArgumentException("This order cannot have a table number");
            }
            
            var card = await _orderManager.DoesUserHasCard(userGuid);
            if (await _orderManager.DoesUserHasOrdered(userGuid))
            {
                throw new ArgumentException("This user has already ordered");
            }
            request.CustomerId = userGuid;
            request.StatusType = request.TypeOfOrder switch
            {
                Order.OrderType.Delivery or Order.OrderType.Online => Order.Status.Pending,
                Order.OrderType.InHouse => Order.Status.Preparing,
                _ => request.StatusType 
            };

            if (request.StatusType == Order.Status.Preparing)
            {
                shouldMakeTicket = true;
            }

            await _orderManager.CreateOrder(request, userGuid, card, shouldMakeTicket);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<List<OrderResponse>> GetOrders()
    {
        try
        {
            var isChef = _roleHelper.IsUserChef();
            var isManager = _roleHelper.IsManagerUser();
            if (!isChef && !isManager)
            {  
                throw new UnauthorizedAccessException();
            }
            return await _orderManager.GetOrders();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<OrderResponse> GetOrderById()
    {
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

            var isCustomer = await _orderManager.DoesCustomerExist(userGuid);
            if (isCustomer == false && !isChef && !isManager)
            {
                throw new UnauthorizedAccessException("you can not get other users orders.");
            }

            return await _orderManager.GetOrderById();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
        
        
    }

    public async Task UpdateOrder(Order order)
    {
        try
        {
            var isChef = _roleHelper.IsUserChef();
            var isManager = _roleHelper.IsManagerUser();
            if (!isChef && !isManager)
            {
                throw new UnauthorizedAccessException("you are not allowed to update this order!");
            }

            await _orderManager.UpdateOrder(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }

    }

    public Task DeleteOrder()
    {
        return _orderManager.DeleteOrder();
    }

    public Task<Order> IsExist(Guid id)
    {
        return _orderManager.IsExist(id);
    }

    public async Task<Ticket> GetTicketById()
    {
        return await _orderManager.GetTicketById();
    }
}