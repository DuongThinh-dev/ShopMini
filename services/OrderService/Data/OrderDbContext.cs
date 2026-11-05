
using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =====================================
        // ORDER CONFIGURATION
        // =====================================

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders", "dbo");

            entity.HasKey(o => o.OrderId);

            entity.Property(o => o.OrderId)
                .HasColumnName("OrderId")
                .ValueGeneratedNever();

            entity.Property(o => o.UserId)
                .HasColumnName("UserId")
                .IsRequired();

            entity.Property(o => o.TotalAmount)
                .HasColumnName("TotalAmount")
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(o => o.ReceiverName)
                .HasColumnName("ReceiverName")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(o => o.Phone)
                .HasColumnName("Phone")
                .HasColumnType("varchar(15)")
                .IsRequired();

            entity.Property(o => o.ShippingAddress)
                .HasColumnName("ShippingAddress")
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(o => o.Status)
                .HasColumnName("Status")
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(o => o.CreatedAt)
                .HasColumnName("CreatedAt")
                .HasColumnType("datetime2(0)")
                .IsRequired();

            entity.Property(o => o.UpdatedAt)
                .HasColumnName("UpdatedAt")
                .HasColumnType("datetime2(0)")
                .IsRequired();

            // Order (1) - OrderItem (N)
            entity.HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index UserId
            entity.HasIndex(o => o.UserId);
        });

        // =====================================
        // ORDER ITEM CONFIGURATION
        // =====================================

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("OrderItems", "dbo");

            entity.HasKey(oi => oi.OrderItemId);

            entity.Property(oi => oi.OrderItemId)
                .ValueGeneratedNever();

            entity.Property(oi => oi.OrderId)
                .IsRequired();

            entity.Property(oi => oi.ProductId)
                .IsRequired();

            entity.Property(oi => oi.ProductName)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(oi => oi.UnitPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(oi => oi.Quantity)
                .IsRequired();

            // entity.Property(oi => oi.Subtotal)
            //     .HasPrecision(18, 2)
            //     .IsRequired();

            entity.Property(oi => oi.Subtotal)
                .HasPrecision(18, 2)
                .HasComputedColumnSql("[UnitPrice] * [Quantity]", stored: true);

            entity.HasIndex(oi => oi.OrderId);
        });
    }
}
