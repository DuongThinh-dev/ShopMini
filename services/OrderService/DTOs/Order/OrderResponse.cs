
using OrderService.Models;

namespace OrderService.DTOs.Order;

public class OrderResponse
{
    public Guid OrderId { get; set; }

    public Guid UserId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public OrderStatus Status { get; set; }

    public string ShippingAddress { get; set; } = string.Empty;

    public string? Note { get; set; }

    public List<OrderItemResponse> OrderItems { get; set; }
        = new();
}
