using Microsoft.AspNetCore.Identity;

namespace RestaurantManagement.Models;

public class Customer: IdentityUser<Guid>
{
    public Customer()
    {
        Id = Guid.NewGuid();
    }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public ICollection<Order> Orders { get; set; }
    public Card Card { get; set; }
}
