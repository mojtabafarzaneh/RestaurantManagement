using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace RestaurantManagement.Models.Configuration;

public class CardConfiguration: IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder
            .HasMany(cart => cart.CartItems)
            .WithOne(cartitem => cartitem.Card)
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