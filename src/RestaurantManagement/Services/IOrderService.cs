using RestaurantManagement.Contracts.Responses.OrderResponse;
using RestaurantManagement.Models;

namespace RestaurantManagement.Services;

public interface IOrderService
{
    public Task CreateOrder(Order request);
    public Task<List<OrderResponse>> GetOrders();
    public Task<OrderResponse> GetOrderById();
    public Task UpdateOrder(Order order);
    public Task DeleteOrder();
    public Task<Order> IsExist(Guid id);
    
    public Task<Ticket> GetTicketById();

}