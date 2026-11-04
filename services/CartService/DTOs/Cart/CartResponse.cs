namespace CartService.DTOs.Cart;

public class CartResponse
{
    public Guid CartId { get; set; }

    public Guid UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public List<CartItemResponse> Items { get; set; }
        = new List<CartItemResponse>();
}