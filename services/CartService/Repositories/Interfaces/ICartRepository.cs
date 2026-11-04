using CartService.Models;

namespace CartService.Repositories.Interfaces;

public interface ICartRepository
{
    // Lấy giỏ hàng theo UserId
    Task<Cart?> GetCartByUserIdAsync(Guid userId);

    // Lấy giỏ hàng theo CartId
    Task<Cart?> GetCartByIdAsync(Guid cartId);

    // Tìm sản phẩm trong giỏ hàng
    Task<CartItem?> GetCartItemAsync(Guid cartId, Guid productId);

    // Tạo giỏ hàng mới
    Task CreateCartAsync(Cart cart);

    // Thêm sản phẩm vào giỏ hàng
    Task AddCartItemAsync(CartItem cartItem);

    // Cập nhật thông tin sản phẩm trong giỏ hàng
    Task UpdateCartItemAsync(CartItem cartItem);

    // Xóa một sản phẩm khỏi giỏ hàng
    Task RemoveCartItemAsync(CartItem cartItem);

    // Xóa toàn bộ sản phẩm trong giỏ hàng
    Task ClearCartAsync(Guid cartId);

    // Lưu thay đổi vào Database
    Task SaveChangesAsync();
}