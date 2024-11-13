using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace RestaurantManagement.Models.Configuration;

public class MenuConfiguration: IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder
            .Property(m => m.Price)
            .HasColumnType("decimal(18, 2)");
        builder
            .Property(m => m.Category)
            .HasConversion<int>();
        builder
            .Property<Guid>("Id")
            .HasColumnType("uniqueidentifier")
            .HasValueGenerator<GuidValueGenerator>();
    }
}