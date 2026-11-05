namespace OrderService.Models;

public class OrderItem
{
    public Guid OrderItemId { get; set; } = Guid.NewGuid();

    public Guid OrderId { get; set; }

    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal Subtotal { get; set; }

    // Navigation Property
    public Order Order { get; set; } = null!;
}