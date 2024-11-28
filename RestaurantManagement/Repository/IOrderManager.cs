using RestaurantManagement.Contracts.Responses.OrderResponse;
using RestaurantManagement.Models;

namespace RestaurantManagement.Repository;

public interface IOrderManager
{
    public Task CreateOrder(Order request);
    public Task<List<OrderResponse>> GetOrders();
    public Task<OrderResponse> GetOrderById(Guid id);
    public Task UpdateOrder(Order order);
    public Task DeleteOrder(Guid id);
    public Task<Order> IsExist(Guid id);
    
}