
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models;
using UserService.Repositories.Interfaces;

namespace UserService.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly UserDbContext _context;

    public RoleRepository(UserDbContext context)
    {
        _context = context;
    }

    // Lấy danh sách tất cả Role
    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        return await _context.Roles.ToListAsync();
    }

    // Tìm Role theo ID
    public async Task<Role?> GetByIdAsync(Guid id)
    {
        return await _context.Roles.FindAsync(id);
    }

    // Thêm Role mới
    public async Task AddAsync(Role role)
    {
        await _context.Roles.AddAsync(role);
    }

    // Cập nhật Role
    public Task UpdateAsync(Role role)
    {
        _context.Roles.Update(role);
        return Task.CompletedTask;
    }

    // Xóa Role
    public Task DeleteAsync(Role role)
    {
        _context.Roles.Remove(role);
        return Task.CompletedTask;
    }

    // Lưu thay đổi vào Database
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
