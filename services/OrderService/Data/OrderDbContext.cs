
using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options)
        : base(options)
    {
    }

    // DbSet đại diện cho các bảng trong database
    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ==============================
        // ORDER CONFIGURATION
        // ==============================

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders");

            entity.HasKey(o => o.OrderId);

            entity.Property(o => o.OrderId)
                .ValueGeneratedNever();

            entity.Property(o => o.UserId)
                .IsRequired();

            entity.Property(o => o.OrderDate)
                .IsRequired()
                .HasColumnType("datetime2");

            entity.Property(o => o.TotalAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            entity.Property(o => o.Status)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(OrderStatus.Pending);

            entity.Property(o => o.ShippingAddress)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(o => o.Note)
                .HasMaxLength(500);
        });

        // ==============================
        // ORDER ITEM CONFIGURATION
        // ==============================

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("OrderItems");

            entity.HasKey(oi => oi.OrderItemId);

            entity.Property(oi => oi.OrderItemId)
                .ValueGeneratedNever();

            entity.Property(oi => oi.OrderId)
                .IsRequired();

            entity.Property(oi => oi.ProductId)
                .IsRequired();

            entity.Property(oi => oi.ProductName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(oi => oi.UnitPrice)
                .IsRequired()
                .HasPrecision(18, 2);

            entity.Property(oi => oi.Quantity)
                .IsRequired();

            entity.Property(oi => oi.Subtotal)
                .IsRequired()
                .HasPrecision(18, 2);

            // Quan hệ Order (1) - OrderItem (N)
            entity.HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Index hỗ trợ tìm kiếm đơn hàng theo UserId
        modelBuilder.Entity<Order>()
            .HasIndex(o => o.UserId);

        // Index hỗ trợ tìm kiếm chi tiết đơn hàng
        modelBuilder.Entity<OrderItem>()
            .HasIndex(oi => oi.OrderId);
    }
}
