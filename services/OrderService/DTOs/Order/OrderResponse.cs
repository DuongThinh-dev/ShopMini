
using OrderService.Models;

namespace OrderService.DTOs.Order;

public class OrderResponse
{
    public Guid OrderId { get; set; }

    public Guid UserId { get; set; }

    public decimal TotalAmount { get; set; }

    public string ReceiverName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string ShippingAddress { get; set; } = string.Empty;

    public OrderStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public List<OrderItemResponse> OrderItems { get; set; }
        = new List<OrderItemResponse>();
}
