using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace RestaurantManagement.Models.Configuration;

public class CartConfiguration: IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder
            .HasMany(cart => cart.CartItems)
            .WithOne(cartitem => cartitem.Cart)
            .HasForeignKey(cartItem => cartItem.CartId);
        builder
            .Property<Guid>("Id")
            .HasColumnType("uniqueidentifier")
            .HasValueGenerator<GuidValueGenerator>();
        builder
            .Property<DateTime>("CreatedOn")
            .HasColumnType("datetime")
            .HasValueGenerator<CreatedAtValueGenerator>();
        builder
            .Property<DateTime>("UpdatedOn")
            .HasColumnType("datetime")
            .HasValueGenerator<CreatedAtValueGenerator>();
    }
}