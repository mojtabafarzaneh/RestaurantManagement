using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagement.Models;

public class Ticket
{
    public Ticket()
    {
        Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public Status TicketStatus { get; set; }
    public bool IsFlagged { get; set; } = false;
    
    //RelationShips
    [ForeignKey(nameof(OrderId))]
    public Guid OrderId { get; set; }
    public Order Order { get; set; }

    public enum Status
    {
        Waiting = 1,
        Served = 2, 
        Delayed = 3,
    }
    
}