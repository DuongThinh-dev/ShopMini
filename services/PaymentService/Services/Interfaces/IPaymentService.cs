
using PaymentService.Models;
using PaymentService.Models.Enums;

namespace PaymentService.Services.Interfaces;

public interface IPaymentService
{
    // Lấy danh sách thanh toán
    Task<IEnumerable<Payment>> GetAllPaymentsAsync();

    // Lấy thanh toán theo ID
    Task<Payment?> GetPaymentByIdAsync(Guid id);

    // Lấy thanh toán theo OrderId
    Task<Payment?> GetPaymentByOrderIdAsync(Guid orderId);

    // Tạo giao dịch thanh toán
    Task<Payment> CreatePaymentAsync(
        Guid orderId,
        decimal amount,
        PaymentMethod paymentMethod
    );

    // Cập nhật trạng thái thanh toán
    Task<Payment> UpdatePaymentStatusAsync(
        Guid paymentId,
        PaymentStatus newStatus,
        string? transactionCode = null,
        string? failureReason = null
    );
}
