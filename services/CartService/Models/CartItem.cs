namespace CartService.Models;

public class CartItem
{
    public Guid CartItemId { get; set; }

    public Guid CartId { get; set; }

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public DateTime AddedAt { get; set; }

    // Navigation Property
    public Cart Cart { get; set; } = null!;
}