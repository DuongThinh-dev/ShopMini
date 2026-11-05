
namespace PaymentService.Models.Enums;

public enum PaymentStatus
{
    Pending = 1,    // Đang chờ thanh toán
    Completed = 2,  // Thanh toán thành công
    Failed = 3,     // Thanh toán thất bại
    Cancelled = 4,  // Thanh toán đã bị hủy
    Refunded = 5    // Đã hoàn tiền
}
