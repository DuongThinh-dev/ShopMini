
using OrderService.Models;

namespace OrderService.Services.Interfaces;

public interface IOrderService
{
    // Lấy tất cả đơn hàng
    Task<List<Order>> GetAllAsync();

    // Lấy đơn hàng theo ID
    Task<Order?> GetByIdAsync(Guid orderId);

    // Lấy danh sách đơn hàng theo UserId
    Task<List<Order>> GetByUserIdAsync(Guid userId);

    // Tạo đơn hàng mới
    Task<Order> CreateAsync(Order order);

    // Cập nhật thông tin đơn hàng
    Task<Order?> UpdateAsync(Order order);

    // Cập nhật trạng thái đơn hàng
    Task<Order?> UpdateStatusAsync(Guid orderId, OrderStatus status);

    // Hủy đơn hàng
    Task<Order?> CancelAsync(Guid orderId, Guid userId);
}
