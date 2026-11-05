
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

    // Lấy tất cả đơn hàng
    public async Task<List<Order>> GetAllAsync()
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    // Lấy đơn hàng theo ID
    public async Task<Order?> GetByIdAsync(Guid orderId)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
    }

    // Lấy danh sách đơn hàng theo UserId
    public async Task<List<Order>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    // Thêm đơn hàng
    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
    }

    // Cập nhật đơn hàng
    public Task UpdateAsync(Order order)
    {
        _context.Orders.Update(order);

        return Task.CompletedTask;
    }

    // Xóa đơn hàng
    public Task DeleteAsync(Order order)
    {
        _context.Orders.Remove(order);

        return Task.CompletedTask;
    }

    // Lưu thay đổi vào database
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
