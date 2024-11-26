using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace RestaurantManagement.Models.Configuration;

public class OrderConfiguration:IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder
            .HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Orders)
            .HasForeignKey(oi => oi.OrderId);
        
        builder
            .HasOne(t => t.Ticket)
            .WithOne(o => o.Order)
            .HasForeignKey<Ticket>(t => t.OrderId);
        
        
        builder
            .Property(o => o.StatusType)
            .HasConversion<int>();

        builder
            .Property(o => o.TypeOfOrder)
            .HasConversion<int>();
        
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