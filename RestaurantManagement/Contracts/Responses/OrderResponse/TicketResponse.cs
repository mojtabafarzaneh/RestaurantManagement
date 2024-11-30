using RestaurantManagement.Models;

namespace RestaurantManagement.Contracts.Responses.OrderResponse;

public class TicketResponse
{
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public bool IsFlagged { get; init; }
    public DateTime CreatedOn { get; init; }
    public Ticket.Status TicketStatus { get; init; }
}