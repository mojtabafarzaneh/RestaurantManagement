using RestaurantManagement.Models;

namespace RestaurantManagement.Contracts.Responses.OrderResponse;

public class OrderResponse
{
    public string Id { get; set; }
    public int? TableNumber { get; set; }
    public Order.OrderType TypeOfOrder { get; set; }
    public Order.Status StatusType { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public Guid CustomerId { get; set; }
    public int EstimatedPrepTime { get;set;}
    public decimal TotalPrice { get;set; }
}