using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagement.Models;

public class Cart
{
    public Cart()
    {
        Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }
    [Required]
    public DateTime CreatedOn { get; set; }
    [Required]
    public DateTime UpdatedOn { get; set; }
    
    //RelationShips
    [ForeignKey(nameof(CustomerId))]
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; }
    public ICollection<CartItem> CartItems { get; set; }
}