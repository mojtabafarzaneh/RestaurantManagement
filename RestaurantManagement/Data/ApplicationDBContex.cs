using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using RestaurantManagement.Models;

namespace RestaurantManagement.Data;

public class ApplicationDBContex: IdentityDbContext<Customer, RestaurantRoles, Guid>
{
    public ApplicationDBContex(DbContextOptions<ApplicationDBContex> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<RestaurantRoles>().HasData(
            new RestaurantRoles
            {
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new RestaurantRoles
            {
                Name = "Customer",
                NormalizedName = "CUSTOMER"
                
            },
            new RestaurantRoles
            {
                Name = "Chef",
                NormalizedName = "CHEF"
            },
            new RestaurantRoles
            {
                Name = "Manager",
                NormalizedName = "MANAGER"
            });
        
        modelBuilder.Entity<Menu>()
            .Property(m => m.Price)
            .HasColumnType("decimal(18, 2)");
    }
    
    public DbSet<Menu> Menus => Set<Menu>();
}
