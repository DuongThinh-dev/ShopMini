
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using OrderService.Repositories.Interfaces;

namespace OrderService.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;

    public OrderRepository(OrderDbContext context)
    {
        _context = context;
    }

    // ==========================================
    // LẤY TẤT CẢ ĐƠN HÀNG
    // ==========================================

    public async Task<List<Order>> GetAllAsync()
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    // ==========================================
    // LẤY ĐƠN HÀNG THEO ID
    // ==========================================

    public async Task<Order?> GetByIdAsync(Guid orderId)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
    }

    // ==========================================
    // LẤY DANH SÁCH ĐƠN HÀNG THEO USER ID
    // ==========================================

    public async Task<List<Order>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    // ==========================================
    // THÊM ĐƠN HÀNG
    // ==========================================

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
    }

    // ==========================================
    // CẬP NHẬT ĐƠN HÀNG
    // ==========================================

    public Task UpdateAsync(Order order)
    {
        _context.Orders.Update(order);

        return Task.CompletedTask;
    }

    // ==========================================
    // XÓA ĐƠN HÀNG
    // ==========================================

    public Task DeleteAsync(Order order)
    {
        _context.Orders.Remove(order);

        return Task.CompletedTask;
    }

    // ==========================================
    // LƯU THAY ĐỔI
    // ==========================================

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
