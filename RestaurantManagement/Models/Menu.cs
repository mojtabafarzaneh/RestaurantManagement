using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagement.Models;

public class Menu
{
    public Menu()
    {
        Id = Guid.NewGuid();
    } 
    [Required]
    Guid Id { get; init; }
    [Required]
    public string Name { get; set; }
    
    public string? Description { get; set; }
    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }
    [Required]
    public CategoryType  Category { get; set; }

    [Required] 
    public int EstimatedPrepTime { get; set; }
    
    [Required]
    public int? QuantityAvailable { get; set; }
    [Required]
    public bool Available { get; set; }
    
    //RelationShips
    public ICollection<OrderItem> OrderItems { get; set; }
    public ICollection<CartItem> CartItems { get; set; }


    public enum CategoryType
    {
        Beverages = 1,
        Appetizers = 2,
        MainCourse = 3,
        Desserts = 4
    }

}