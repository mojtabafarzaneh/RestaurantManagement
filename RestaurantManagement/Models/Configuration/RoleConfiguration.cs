using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RestaurantManagement.Models.Configuration;

public class RoleConfiguration: IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole
            {
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new IdentityRole
            {
                Name = "Customer",
                NormalizedName = "CUSTOMER"
            },
            new IdentityRole
            {
                Name = "Chef",
                NormalizedName = "CHEF"
            },
            new IdentityRole
            {
                Name = "Manager",
                NormalizedName = "MANAGER"
            });
    }
}