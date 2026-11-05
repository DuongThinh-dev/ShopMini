
using Microsoft.EntityFrameworkCore;
using PaymentService.Models;

namespace PaymentService.Data;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
        : base(options)
    {
    }

    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("Payments");

            entity.HasKey(p => p.PaymentId);

            entity.Property(p => p.PaymentId)
                .HasDefaultValueSql("NEWSEQUENTIALID()")
                .ValueGeneratedOnAdd();

            entity.Property(p => p.OrderId)
                .IsRequired();

            entity.Property(p => p.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            // PaymentMethod -> Method
            entity.Property(p => p.PaymentMethod)
                .HasColumnName("Method")
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            // PaymentStatus -> Status
            entity.Property(p => p.PaymentStatus)
                .HasColumnName("Status")
                .HasConversion<string>()
                .HasMaxLength(30)
                .HasDefaultValueSql("(N'Pending')")
                .IsRequired();

            entity.Property(p => p.TransactionCode)
                .HasMaxLength(100)
                .IsRequired(false);

            entity.Property(p => p.CreatedAt)
                .HasColumnType("datetime2(0)")
                .HasDefaultValueSql("SYSUTCDATETIME()")
                .ValueGeneratedOnAdd();

            entity.Property(p => p.UpdatedAt)
                .HasColumnType("datetime2(0)")
                .HasDefaultValueSql("SYSUTCDATETIME()")
                .ValueGeneratedOnAdd();

            // Mỗi đơn hàng chỉ có một giao dịch
            entity.HasIndex(p => p.OrderId)
                .IsUnique()
                .HasDatabaseName("UX_Payments_OrderId");

            // TransactionCode duy nhất nếu khác NULL
            entity.HasIndex(p => p.TransactionCode)
                .IsUnique()
                .HasFilter("[TransactionCode] IS NOT NULL")
                .HasDatabaseName("UX_Payments_TransactionCode");
        });
    }
}
