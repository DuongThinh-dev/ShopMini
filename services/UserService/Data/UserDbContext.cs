using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Data;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options)
    {
    }

    // Khai báo các bảng trong Database
    public DbSet<User> Users { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<Address> Addresses { get; set; }

    // Cấu hình ánh xạ Models với SQL Server
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. ROLE

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");

            entity.HasKey(r => r.RoleId);

            entity.Property(r => r.RoleId)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            entity.Property(r => r.RoleName)
                .HasMaxLength(30)
                .IsRequired();

            entity.HasIndex(r => r.RoleName)
                .IsUnique()
                .HasDatabaseName("UQ_Roles_RoleName");
        });

        // 2. USER

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(u => u.UserId);

            entity.Property(u => u.UserId)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            entity.Property(u => u.FullName)
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(u => u.Email)
                .HasMaxLength(256)
                .IsRequired();

            entity.Property(u => u.PasswordHash)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(u => u.RoleId)
                .IsRequired();

            entity.Property(u => u.IsActive)
                .HasDefaultValue(true);

            entity.Property(u => u.CreatedAt)
                .HasColumnType("datetime2(0)")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.Property(u => u.UpdatedAt)
                .HasColumnType("datetime2(0)")
                .IsRequired(false);

            // Email không được trùng
            entity.HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("UQ_Users_Email");

            // Quan hệ User - Role
            entity.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // 3. ADDRESS

        modelBuilder.Entity<Address>(entity =>
        {
            entity.ToTable("Addresses");

            entity.HasKey(a => a.AddressId);

            entity.Property(a => a.AddressId)
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            entity.Property(a => a.ReceiverName)
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(a => a.Phone)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(a => a.Province)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(a => a.District)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(a => a.Ward)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(a => a.DetailAddress)
                .HasMaxLength(300)
                .IsRequired();

            entity.Property(a => a.IsDefault)
                .HasDefaultValue(false);

            // Quan hệ User - Address
            entity.HasOne(a => a.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Mỗi User chỉ có tối đa một địa chỉ mặc định
            entity.HasIndex(a => a.UserId)
                .HasFilter("[IsDefault] = 1")
                .IsUnique()
                .HasDatabaseName("UX_Addresses_OneDefaultPerUser");

            // Index hỗ trợ tìm kiếm địa chỉ theo UserId
            entity.HasIndex(a => a.UserId)
                .HasDatabaseName("IX_Addresses_UserId");
        });
    }
}
