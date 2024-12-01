using System.ComponentModel.DataAnnotations;
using RestaurantManagement.Models;

namespace RestaurantManagement.Contracts.Requests.OrderRequest;

public class UpdateOrderRequest
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public Order.Status StatusType { get;set; }
    
}