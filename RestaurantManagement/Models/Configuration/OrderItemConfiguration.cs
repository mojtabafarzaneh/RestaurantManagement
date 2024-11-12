using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RestaurantManagement.Models.Configuration;

public class OrderItemConfiguration:IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder
            .HasOne(oi => oi.Menus)
            .WithMany(m => m.OrderItems)
            .HasForeignKey(oi => oi.MenuId);
        
        builder
            .Property(oi => oi.Price)
            .HasColumnType("decimal(18,2)");
        
    }
}