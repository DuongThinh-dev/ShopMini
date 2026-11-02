using Microsoft.EntityFrameworkCore;
using ProductService.Models;

namespace ProductService.Data;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options)
        : base(options)
    {
    }

    // Khai báo các bảng trong Database
    public DbSet<Category> Categories { get; set; }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =========================================
        // 1. Cấu hình bảng Categories
        // =========================================

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Categories");

            entity.HasKey(c => c.CategoryId);

            entity.Property(c => c.CategoryId)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            entity.Property(c => c.CategoryName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(c => c.Description)
                .HasMaxLength(500)
                .IsRequired(false);

            entity.Property(c => c.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            // Tên danh mục không được trùng
            entity.HasIndex(c => c.CategoryName)
                .IsUnique()
                .HasDatabaseName("UQ_Categories_CategoryName");
        });

        // =========================================
        // 2. Cấu hình bảng Products
        // =========================================

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products", table =>
            {
                table.HasCheckConstraint(
                    "CK_Products_Price",
                    "[Price] > 0");

                table.HasCheckConstraint(
                    "CK_Products_Stock",
                    "[Stock] >= 0");
            });

            entity.HasKey(p => p.ProductId);

            entity.Property(p => p.ProductId)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            entity.Property(p => p.CategoryId)
                .IsRequired();

            entity.Property(p => p.ProductName)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(p => p.Description)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            entity.Property(p => p.ImageUrl)
                .HasMaxLength(500)
                .IsRequired(false);

            entity.Property(p => p.Price)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(p => p.Stock)
                .HasDefaultValue(0)
                .IsRequired();

            entity.Property(p => p.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            entity.Property(p => p.CreatedAt)
                .HasColumnType("datetime2(0)")
                .HasDefaultValueSql("SYSUTCDATETIME()")
                .IsRequired();

            entity.Property(p => p.UpdatedAt)
                .HasColumnType("datetime2(0)")
                .HasDefaultValueSql("SYSUTCDATETIME()")
                .IsRequired();

            // Tạo Index cho CategoryId
            entity.HasIndex(p => p.CategoryId)
                .HasDatabaseName("IX_Products_CategoryId");

            // Thiết lập quan hệ Product - Category
            entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Products_Categories");
        });
    }
}