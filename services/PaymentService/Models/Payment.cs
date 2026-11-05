
using PaymentService.Models.Enums;

namespace PaymentService.Models;

public class Payment
{
    // Khóa chính
    public Guid PaymentId { get; set; }

    // ID đơn hàng từ OrderService
    public Guid OrderId { get; set; }

    // Số tiền thanh toán
    public decimal Amount { get; set; }

    // Phương thức thanh toán
    public PaymentMethod PaymentMethod { get; set; }

    // Trạng thái thanh toán
    public PaymentStatus PaymentStatus { get; set; }

    // Mã giao dịch từ cổng thanh toán (nếu có)
    public string? TransactionCode { get; set; }

    // Lý do thanh toán thất bại (nếu có)
    public string? FailureReason { get; set; }

    // Thời điểm tạo giao dịch
    public DateTime CreatedAt { get; set; }

    // Thời điểm cập nhật gần nhất
    public DateTime? UpdatedAt { get; set; }

    // Thời điểm thanh toán thành công
    public DateTime? PaidAt { get; set; }
}
