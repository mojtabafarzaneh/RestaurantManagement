using RestaurantManagement.Models;

namespace RestaurantManagement.Contracts.Responses.OrderResponse;

public class TicketResponse
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public bool IsFlagged { get; set; }
    public DateTime CreatedOn { get; set; }
    public Ticket.Status TicketStatus { get; set; }
}