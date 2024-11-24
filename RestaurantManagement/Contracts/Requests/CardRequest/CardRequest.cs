using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.Contracts.Requests.CardRequest;

public class CardRequest
{
    [Required]
    public Guid CustomerId { get; set; }
}