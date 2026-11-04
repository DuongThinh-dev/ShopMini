namespace CartService.Models;

public class Cart
{
    public Guid CartId { get; set; }

    public Guid UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Navigation Property
    public ICollection<CartItem> CartItems { get; set; }
        = new List<CartItem>();
}