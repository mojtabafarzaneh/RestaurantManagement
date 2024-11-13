using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace RestaurantManagement.Models.Configuration;

public class CartItemConfiguration: IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder
            .HasOne(ci => ci.Menu)
            .WithMany(menu => menu.CartItems)
            .HasForeignKey(ci => ci.MenuId);
        builder
            .Property<Guid>("Id")
            .HasColumnType("uniqueidentifier")
            .HasValueGenerator<GuidValueGenerator>();
    }
}