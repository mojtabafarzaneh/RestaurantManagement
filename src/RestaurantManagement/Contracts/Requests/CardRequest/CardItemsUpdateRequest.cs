using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.Contracts.Requests.CardRequest;

public class CardItemsUpdateRequest
{
    [Required]
    public Guid MenuId { get; set; }
    [Required]
    public int Quantity { get; set; }
}