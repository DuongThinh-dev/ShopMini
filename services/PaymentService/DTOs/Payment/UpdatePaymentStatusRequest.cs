
using System.ComponentModel.DataAnnotations;
using PaymentService.Models.Enums;

namespace PaymentService.DTOs.Payment;

public class UpdatePaymentStatusRequest
{
    // Trạng thái thanh toán mới
    [Required]
    [EnumDataType(
        typeof(PaymentStatus),
        ErrorMessage = "Trạng thái thanh toán không hợp lệ."
    )]
    public PaymentStatus PaymentStatus { get; set; }

    // Mã giao dịch từ cổng thanh toán
    [StringLength(
        100,
        ErrorMessage = "TransactionCode không được vượt quá 100 ký tự."
    )]
    public string? TransactionCode { get; set; }
}
