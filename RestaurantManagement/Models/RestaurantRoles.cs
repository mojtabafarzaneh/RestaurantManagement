using Microsoft.AspNetCore.Identity;

namespace RestaurantManagement.Models;

public class RestaurantRoles: IdentityRole<Guid>
{
    public RestaurantRoles()
    {
        Id = Guid.NewGuid();
    }
}