using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagement.Models;

public class Order
{
    public Order()
    {
        Id = Guid.NewGuid();
    }
    [Required]
    public Guid Id { get; set; }

    public int? TableNumber { get; set; }
    [Required]
    public OrderType TypeOfOrder { get; set; }
    [Required]
    public Status StatusType {get; set;}
    
    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalPrice { get; set; }
    [Required]
    //should be populated by the estimatedPrepTime timespan
    public DateTime EstimatedReadyTime { get; set; }
    
    [Required]
    public DateTime CreatedOn { get; set; }
    [Required]
    public DateTime UpdatedOn { get; set; }
    
    //RelationShips
    //Many-To-One
    [ForeignKey(nameof(CustomerId))]
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; }
    //One-To-Many
    public ICollection<OrderItem> OrderItems { get; set; }
    //One-To-One
    public Ticket Ticket { get; set; }
    
    //Enums
    public enum OrderType
    {
        Online = 1,
        InHouse = 2,
        Delivery = 3,
        
    }

    public enum Status
    {
        Pending = 1,
        Delayed =2, 
        Preparing = 3,
        Completed = 4,
        Cancelled = 5,
        Delivered = 6,
    }
}
