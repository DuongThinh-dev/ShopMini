using CartService.Models;

namespace CartService.Services.Interfaces;

public interface ICartService
{
    // Lấy giỏ hàng theo UserId
    Task<Cart> GetCartAsync(Guid userId);

    // Thêm sản phẩm vào giỏ hàng
    Task<Cart> AddToCartAsync(
        Guid userId,
        Guid productId,
        int quantity
    );

    // Cập nhật số lượng sản phẩm
    Task<Cart> UpdateCartItemAsync(
        Guid userId,
        Guid productId,
        int quantity
    );

    // Xóa một sản phẩm khỏi giỏ hàng
    Task RemoveCartItemAsync(
        Guid userId,
        Guid productId
    );

    // Xóa toàn bộ sản phẩm trong giỏ hàng
    Task ClearCartAsync(Guid userId);
}