
using System.ComponentModel.DataAnnotations;

namespace OrderService.DTOs.Order;

public class CreateOrderRequest
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(500)]
    public string ShippingAddress { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Note { get; set; }

    [Required]
    [MinLength(1)]
    public List<CreateOrderItemRequest> OrderItems { get; set; }
        = new();
}
