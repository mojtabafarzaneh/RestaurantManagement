using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RestaurantManagement.Models.Configuration;

public class CartConfiguration: IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder
            .HasMany(cart => cart.CartItems)
            .WithOne(cartitem => cartitem.Cart)
            .HasForeignKey(cartItem => cartItem.CartId);
    }
}