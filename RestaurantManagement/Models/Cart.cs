using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagement.Models;

public class Cart
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    public DateTime CreatedOn { get; set; } = DateTime.Now;
    [Required]
    public DateTime UpdatedOn { get; set; } = DateTime.Now;
    
    //RelationShips
    [ForeignKey(nameof(CustomerId))]
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; }
    public ICollection<CartItem> CartItems { get; set; }
}