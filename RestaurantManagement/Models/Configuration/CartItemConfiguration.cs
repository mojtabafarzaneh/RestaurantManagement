using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RestaurantManagement.Models.Configuration;

public class CartItemConfiguration: IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder
            .HasOne(ci => ci.Menu)
            .WithMany(menu => menu.CartItems)
            .HasForeignKey(ci => ci.MenuId);
    }
}