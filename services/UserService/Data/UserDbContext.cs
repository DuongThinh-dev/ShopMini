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
}
