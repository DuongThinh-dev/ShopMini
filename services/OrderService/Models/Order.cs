
namespace OrderService.Models;

public class Order
{
    public Guid OrderId { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }

    public decimal TotalAmount { get; set; }

    public string ReceiverName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string ShippingAddress { get; set; } = string.Empty;

    public OrderStatus Status { get; set; } = OrderStatus.PendingPayment;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Property
    public ICollection<OrderItem> OrderItems { get; set; }
        = new List<OrderItem>();
}
