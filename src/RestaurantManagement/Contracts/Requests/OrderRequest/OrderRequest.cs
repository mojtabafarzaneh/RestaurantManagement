using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RestaurantManagement.Models;

namespace RestaurantManagement.Contracts.Requests.OrderRequest;

public class OrderRequest
{
    [Required] 
    public Order.OrderType TypeOfOrder { get; init; }

    public int? TableNumber { get; init; }
    
}