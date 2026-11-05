namespace OrderService.Models;

public class Order
{
    public Guid OrderId { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public decimal TotalAmount { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public string ShippingAddress { get; set; } = string.Empty;

    public string? Note { get; set; }

    // Navigation Property
    public ICollection<OrderItem> OrderItems { get; set; }
        = new List<OrderItem>();
}