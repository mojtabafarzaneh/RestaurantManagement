using RestaurantManagement.Contracts.Responses.OrderResponse;
using RestaurantManagement.Helper;
using RestaurantManagement.Models;
using RestaurantManagement.Repository;

namespace RestaurantManagement.Services;

public class OrderService: IOrderService
{
    private readonly IOrderManager _orderManager;
    private readonly ILogger<OrderService> _logger;
    private readonly RoleService _roleService;
    private readonly UserService _userService;

    public OrderService(IOrderManager orderManager,
        ILogger<OrderService> logger,
        RoleService roleService, UserService userService)
    {
        _orderManager = orderManager;
        _logger = logger;
        _roleService = roleService;
        _userService = userService;
    }

    public async Task CreateOrder(Order request)
    {
        try
        {
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

            await _orderManager.CreateOrder(request);
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
            var isChef = _roleService.IsUserChef();
            var isManager = _roleService.IsManagerUser();
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
            var userId = _userService.UserId();
            var isChef = _roleService.IsUserChef();
            var isManager = _roleService.IsManagerUser();

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
            var isChef = _roleService.IsUserChef();
            var isManager = _roleService.IsManagerUser();
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