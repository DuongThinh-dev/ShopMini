namespace CartService.DTOs.Cart;

public class CartItemResponse
{
    public Guid CartItemId { get; set; }

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public DateTime AddedAt { get; set; }
}