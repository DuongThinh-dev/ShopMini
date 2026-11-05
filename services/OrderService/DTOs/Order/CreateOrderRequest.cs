using System.ComponentModel.DataAnnotations;

namespace OrderService.DTOs.Order;

public class CreateOrderRequest
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string ReceiverName { get; set; } = string.Empty;

    [Required]
    [MaxLength(15)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string ShippingAddress { get; set; } = string.Empty;

    public List<CreateOrderItemRequest> OrderItems { get; set; }
        = new();
}