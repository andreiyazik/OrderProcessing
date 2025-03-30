using Microsoft.EntityFrameworkCore;
using OrderProcessing.Infrastructure.Entities;

namespace OrderProcessing.Infrastructure;

public class OrderDbContext : DbContext
{
    public DbSet<OrderEntity> Orders { get; set; }
    public DbSet<OrderItemEntity> OrderItems { get; set; }
    public DbSet<InventoryItemEntity> InventoryItems { get; set; }
    public DbSet<InventoryReservationEntity> InventoryReservations { get; set; }

    public OrderDbContext(DbContextOptions<OrderDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderEntity>()
            .HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId);

        modelBuilder.Entity<OrderItemEntity>()
            .HasOne(i => i.InventoryItem);

        modelBuilder.Entity<InventoryItemEntity>()
            .HasMany(o => o.Reservations)
            .WithOne(i => i.InventoryItem)
            .HasForeignKey(i => i.InventoryItemId);

        modelBuilder.Entity<InventoryReservationEntity>()
            .HasOne(i => i.Order)
            .WithMany()
            .HasForeignKey(i => i.OrderId);
    }
}
