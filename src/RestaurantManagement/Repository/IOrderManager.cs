using RestaurantManagement.Contracts.Responses.OrderResponse;
using RestaurantManagement.Models;

namespace RestaurantManagement.Repository;

public interface IOrderManager
{
    public Task<bool> DoesUserHasOrdered(Guid id);
    public Task<List<Order>> DoesAnyOrdersExist();
    public Task<Order?> GetOrderById(Guid id);
    public Task<Card> DoesUserHasCard(Guid id);
    public Task CreateOrder(Order request, Guid id, Card card, bool shouldMakeTicket);
    public Task<List<OrderResponse>> GetOrders(List<Order> orders);
    public Task<Order?> FindOrderById(Guid id);
    public Task RemoveTicket(Ticket ticket);
    public Task<Ticket?> FindTicket(Guid id);
    public Task<OrderResponse> GetOrderById(Order order);
    public Task UpdateOrder(Order order);
    public Task DeleteOrder(Order order);
    public Task<Order> IsExist(Guid id);
    
    public Task<Ticket> GetTicketById(Guid id);

}