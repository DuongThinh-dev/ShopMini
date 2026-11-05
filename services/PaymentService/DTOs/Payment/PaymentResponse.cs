
using PaymentService.Models.Enums;

namespace PaymentService.DTOs.Payment;

public class PaymentResponse
{
    // ID thanh toán
    public Guid PaymentId { get; set; }

    // ID đơn hàng
    public Guid OrderId { get; set; }

    // Số tiền thanh toán
    public decimal Amount { get; set; }

    // Phương thức thanh toán
    public PaymentMethod PaymentMethod { get; set; }

    // Trạng thái thanh toán
    public PaymentStatus PaymentStatus { get; set; }

    // Mã giao dịch
    public string? TransactionCode { get; set; }

    // Lý do thanh toán thất bại
    public string? FailureReason { get; set; }

    // Thời điểm tạo giao dịch
    public DateTime CreatedAt { get; set; }

    // Thời điểm cập nhật
    public DateTime? UpdatedAt { get; set; }

    // Thời điểm thanh toán thành công
    public DateTime? PaidAt { get; set; }

    // Chuyển đổi từ Entity sang Response DTO
    public static PaymentResponse FromEntity(
        global::PaymentService.Models.Payment payment)
    {
        return new PaymentResponse
        {
            PaymentId = payment.PaymentId,
            OrderId = payment.OrderId,
            Amount = payment.Amount,
            PaymentMethod = payment.PaymentMethod,
            PaymentStatus = payment.PaymentStatus,
            TransactionCode = payment.TransactionCode,
            FailureReason = payment.FailureReason,
            CreatedAt = payment.CreatedAt,
            UpdatedAt = payment.UpdatedAt,
            PaidAt = payment.PaidAt
        };
    }
}
