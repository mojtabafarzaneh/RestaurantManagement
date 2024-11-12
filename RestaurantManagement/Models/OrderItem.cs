using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagement.Models;

public class OrderItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int Quantity { get; set; }
    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }
    
    //RelationShips
    [ForeignKey(nameof(MenuId))]
    public Guid MenuId { get; set; }
    [ForeignKey(nameof(OrderId))]
    public Guid OrderId { get; set; }
    
    public Menu Menus { get; set; }
    public Order Orders { get; set; }
}