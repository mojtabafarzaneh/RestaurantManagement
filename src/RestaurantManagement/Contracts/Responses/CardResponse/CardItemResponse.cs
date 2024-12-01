namespace RestaurantManagement.Contracts.Responses.CardResponse;

public class CardItemResponse
{
    public Guid Id { get; set; }
    public Guid MenuId { get; set; }
    public Guid CartId { get; set; }
    public int Quantity { get; set; }
}