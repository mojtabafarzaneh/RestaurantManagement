using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RestaurantManagement.Models.Configuration;

public class RoleConfiguration: IEntityTypeConfiguration<RestaurantRoles>
{
    public void Configure(EntityTypeBuilder<RestaurantRoles> modelBuilder)
    {
        modelBuilder.HasData(
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
    }
}