
using OrderService.Models;

namespace OrderService.Repositories.Interfaces;

public interface IOrderRepository
{
    // Lấy tất cả đơn hàng
    Task<List<Order>> GetAllAsync();

    // Lấy đơn hàng theo ID
    Task<Order?> GetByIdAsync(Guid orderId);

    // Lấy danh sách đơn hàng theo UserId
    Task<List<Order>> GetByUserIdAsync(Guid userId);

    // Thêm đơn hàng
    Task AddAsync(Order order);

    // Cập nhật đơn hàng
    Task UpdateAsync(Order order);

    // Xóa đơn hàng
    Task DeleteAsync(Order order);

    // Lưu thay đổi vào database
    Task SaveChangesAsync();
}
