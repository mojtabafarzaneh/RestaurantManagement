using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
            .Property(o => o.TotalPrice)
            .HasColumnType("decimal(18,2)");
        
        builder
            .Property(o => o.StatusType)
            .HasConversion<int>();

        builder
            .Property(o => o.TypeOfOrder)
            .HasConversion<int>();
    }
}