using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Models;
using RestaurantManagement.Models.Configuration;

namespace RestaurantManagement.Data;

public class ApplicationDBContex: IdentityDbContext<Customer, RestaurantRoles, Guid>
{
    public ApplicationDBContex(DbContextOptions<ApplicationDBContex> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
        modelBuilder.ApplyConfiguration(new MenuConfiguration());
        modelBuilder.ApplyConfiguration(new CardConfiguration());
        modelBuilder.ApplyConfiguration(new CardItemConfiguration());
        modelBuilder.ApplyConfiguration(new TicketConfiguration());
    }
    
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Card> Cards => Set<Card>();
    public DbSet<CardItem> CardItems => Set<CardItem>();
    
}
