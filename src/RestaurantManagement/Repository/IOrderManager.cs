using RestaurantManagement.Contracts.Responses.OrderResponse;
using RestaurantManagement.Models;

namespace RestaurantManagement.Repository;

public interface IOrderManager
{
    public Task<bool> DoesUserHasOrdered(Guid id);
    public Task<Card> DoesUserHasCard(Guid id);
    public Task CreateOrder(Order request, Guid id, Card card, bool shouldMakeTicket);
    public Task<List<OrderResponse>> GetOrders();
    public Task<OrderResponse> GetOrderById();
    public Task UpdateOrder(Order order);
    public Task DeleteOrder();
    public Task<Order> IsExist(Guid id);
    
    public Task<Ticket> GetTicketById();
    public Task<bool> DoesCustomerExist(Guid id);

}