using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace RestaurantManagement.Models.Configuration;

public class RoleConfiguration: IEntityTypeConfiguration<RestaurantRoles>
{
    public void Configure(EntityTypeBuilder<RestaurantRoles> modelBuilder)
    {
     modelBuilder
      .Property<Guid>("Id")
      .HasColumnType("uniqueidentifier")
      .HasValueGenerator<GuidValueGenerator>();
     
        // modelBuilder.HasData(
        //     new RestaurantRoles
        //     {
        //         Name = "Admin",
        //         NormalizedName = "ADMIN"
        //     },
        //     new RestaurantRoles
        //    {
        //         Name = "User",
        //         NormalizedName = "USER"
        //         
        //     },
        //     new RestaurantRoles
        //     {
        //         Name = "Chef",
        //         NormalizedName = "CHEF"
        //     },
        //     new RestaurantRoles
        //     {
        //         Name = "Manager",
        //         NormalizedName = "MANAGER"
        //     });
    }
}