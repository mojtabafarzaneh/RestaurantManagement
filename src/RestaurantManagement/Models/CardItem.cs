using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagement.Models;

public class CardItem
{
    public CardItem()
    {
        Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }
    //Quantity of this item in the cart
    public int Quantity { get; set; }
    
    //RelationShips
    [ForeignKey(nameof(CartId))] 
    public Guid CartId { get; set; }
    [ForeignKey(nameof(MenuId))]
    public Guid MenuId { get; set; }
    
    public Menu Menu { get; set; }
    public Card Card { get; set; }
}