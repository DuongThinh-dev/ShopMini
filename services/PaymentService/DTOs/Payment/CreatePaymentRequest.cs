
using System.ComponentModel.DataAnnotations;
using PaymentService.Models.Enums;

namespace PaymentService.DTOs.Payment;

public class CreatePaymentRequest
{
    // ID đơn hàng
    [Required]
    public Guid OrderId { get; set; }

    // Số tiền thanh toán
    [Required]
    [Range(typeof(decimal), "0.01", "9999999999999999.99",
        ErrorMessage = "Số tiền thanh toán phải lớn hơn 0.")]
    public decimal Amount { get; set; }

    // Phương thức thanh toán
    [Required]
    [EnumDataType(typeof(PaymentMethod),
        ErrorMessage = "Phương thức thanh toán không hợp lệ.")]
    public PaymentMethod PaymentMethod { get; set; }
}
