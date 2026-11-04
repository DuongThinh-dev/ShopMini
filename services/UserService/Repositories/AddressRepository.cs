
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models;
using UserService.Repositories.Interfaces;

namespace UserService.Repositories;

public class AddressRepository : IAddressRepository
{
    private readonly UserDbContext _context;

    public AddressRepository(UserDbContext context)
    {
        _context = context;
    }

    // Lấy tất cả địa chỉ của một User
    public async Task<IEnumerable<Address>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Addresses
            .Where(a => a.UserId == userId)
            .ToListAsync();
    }

    // Lấy địa chỉ theo ID
    public async Task<Address?> GetByIdAsync(Guid id)
    {
        return await _context.Addresses
            .FirstOrDefaultAsync(a => a.AddressId == id);
    }

    // Thêm địa chỉ
    public async Task AddAsync(Address address)
    {
        await _context.Addresses.AddAsync(address);
    }

    // Cập nhật địa chỉ
    public Task UpdateAsync(Address address)
    {
        _context.Addresses.Update(address);
        return Task.CompletedTask;
    }

    // Xóa địa chỉ
    public Task DeleteAsync(Address address)
    {
        _context.Addresses.Remove(address);
        return Task.CompletedTask;
    }

    // Lưu thay đổi
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
