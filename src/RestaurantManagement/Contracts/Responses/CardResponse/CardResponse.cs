namespace RestaurantManagement.Contracts.Responses.CardResponse;

public class CardResponse
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
}