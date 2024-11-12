using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RestaurantManagement.Models.Configuration;

public class CustomerConfiguration: IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder
            .HasMany(c => c.Orders)
            .WithOne(o => o.Customer)
            .HasForeignKey(o=>o.CustomerId);
        
        builder
            .HasOne(c => c.Cart)
            .WithOne(cart => cart.Customer)
            .HasForeignKey<Cart>(cart => cart.CustomerId);

    }
}