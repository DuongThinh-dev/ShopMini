
using PaymentService.Models;

namespace PaymentService.Repositories.Interfaces;

public interface IPaymentRepository
{
    // Lấy tất cả thanh toán
    Task<IEnumerable<Payment>> GetAllAsync();

    // Lấy thanh toán theo ID
    Task<Payment?> GetByIdAsync(Guid id);

    // Lấy thanh toán theo OrderId
    Task<Payment?> GetByOrderIdAsync(Guid orderId);

    // Thêm thanh toán mới
    Task AddAsync(Payment payment);

    // Cập nhật thanh toán
    void Update(Payment payment);

    // Lưu thay đổi vào database
    Task SaveChangesAsync();
}
