
using System.ComponentModel.DataAnnotations;

namespace OrderService.DTOs.Order;

public class UpdateOrderRequest
{
    [Required]
    [MaxLength(500)]
    public string ShippingAddress { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Note { get; set; }
}
