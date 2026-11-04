using System.ComponentModel.DataAnnotations;

namespace CartService.DTOs.Cart;

public class UpdateCartItemRequest
{
    [Range(1, int.MaxValue,
        ErrorMessage = "Quantity must be greater than zero.")]
    public int Quantity { get; set; }
}