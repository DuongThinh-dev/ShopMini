
using Microsoft.EntityFrameworkCore;
using PaymentService.Models;
using PaymentService.Models.Enums;

namespace PaymentService.Data;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(
        DbContextOptions<PaymentDbContext> options)
        : base(options)
    {
    }

    // Khai báo bảng Payments
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Payment>(entity =>
        {
            // Tên bảng
            entity.ToTable("Payments", table =>
            {
                table.HasCheckConstraint(
                    "CK_Payments_Amount",
                    "[Amount] > 0"
                );
            });

            // Khóa chính
            entity.HasKey(p => p.PaymentId);

            // PaymentId
            entity.Property(p => p.PaymentId)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            // OrderId
            entity.Property(p => p.OrderId)
                .IsRequired();

            // Amount
            entity.Property(p => p.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            // PaymentMethod
            entity.Property(p => p.PaymentMethod)
                .HasConversion<int>()
                .IsRequired();

            // PaymentStatus
            entity.Property(p => p.PaymentStatus)
                .HasConversion<int>()
                .HasDefaultValue(PaymentStatus.Pending)
                .IsRequired();

            // TransactionCode
            entity.Property(p => p.TransactionCode)
                .HasMaxLength(100)
                .IsUnicode()
                .IsRequired(false);

            // FailureReason
            entity.Property(p => p.FailureReason)
                .HasMaxLength(500)
                .IsUnicode()
                .IsRequired(false);

            // CreatedAt
            entity.Property(p => p.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()")
                .IsRequired();

            // UpdatedAt
            entity.Property(p => p.UpdatedAt)
                .HasColumnType("datetime2")
                .IsRequired(false);

            // PaidAt
            entity.Property(p => p.PaidAt)
                .HasColumnType("datetime2")
                .IsRequired(false);

            // Index tìm kiếm theo OrderId
            entity.HasIndex(p => p.OrderId)
                .HasDatabaseName("IX_Payments_OrderId");

            // Không cho phép trùng TransactionCode
            // khi TransactionCode có giá trị
            entity.HasIndex(p => p.TransactionCode)
                .IsUnique()
                .HasFilter("[TransactionCode] IS NOT NULL")
                .HasDatabaseName("UX_Payments_TransactionCode");
        });
    }
}
