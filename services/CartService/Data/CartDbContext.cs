using Microsoft.EntityFrameworkCore;
using CartService.Models;

namespace CartService.Data;

public class CartDbContext : DbContext
{
    public CartDbContext(DbContextOptions<CartDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(c => c.CartId);

            entity.HasIndex(c => c.UserId)
                .IsUnique();

            entity.HasMany(c => c.CartItems)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId);
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(ci => ci.CartItemId);

            entity.HasIndex(ci => new
            {
                ci.CartId,
                ci.ProductId
            }).IsUnique();

            entity.Property(ci => ci.Quantity)
                .IsRequired();
        });
    }
}