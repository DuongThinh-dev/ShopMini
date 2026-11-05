
namespace OrderService.DTOs.Order;

public class OrderItemResponse
{
    public Guid OrderItemId { get; set; }

    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal Subtotal { get; set; }
}
