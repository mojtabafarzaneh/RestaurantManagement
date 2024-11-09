using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
    
}