using CartService.Models;
using CartService.Repositories.Interfaces;
using CartService.Services.Interfaces;

namespace CartService.Services.Implementations;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;

    public CartService(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    // Lấy giỏ hàng theo UserId
    public async Task<Cart> GetCartAsync(Guid userId)
    {
        ValidateUserId(userId);

        return await GetOrCreateCartAsync(userId);
    }

    // Thêm sản phẩm vào giỏ hàng
    public async Task<Cart> AddToCartAsync(
        Guid userId,
        Guid productId,
        int quantity)
    {
        ValidateUserId(userId);
        ValidateProductId(productId);
        ValidateQuantity(quantity);

        // Lấy hoặc tạo giỏ hàng
        var cart = await GetOrCreateCartAsync(userId);

        // Kiểm tra sản phẩm đã tồn tại trong giỏ hàng chưa
        var existingItem = await _cartRepository.GetCartItemAsync(
            cart.CartId,
            productId
        );

        if (existingItem != null)
        {
            // Nếu đã tồn tại thì tăng số lượng
            existingItem.Quantity += quantity;

            await _cartRepository.UpdateCartItemAsync(existingItem);
        }
        else
        {
            // Nếu chưa tồn tại thì tạo CartItem mới
            var cartItem = new CartItem
            {
                CartItemId = Guid.NewGuid(),
                CartId = cart.CartId,
                ProductId = productId,
                Quantity = quantity
                // AddedAt = DateTime.UtcNow
            };

            await _cartRepository.AddCartItemAsync(cartItem);

            cart.CartItems.Add(cartItem);
        }

        cart.UpdatedAt = DateTime.UtcNow;

        await _cartRepository.SaveChangesAsync();

        return cart;
    }

    // Cập nhật số lượng sản phẩm
    public async Task<Cart> UpdateCartItemAsync(
        Guid userId,
        Guid productId,
        int quantity)
    {
        ValidateUserId(userId);
        ValidateProductId(productId);
        ValidateQuantity(quantity);

        // Tìm giỏ hàng
        var cart = await _cartRepository.GetCartByUserIdAsync(userId);

        if (cart == null)
        {
            throw new KeyNotFoundException(
                "Cart not found."
            );
        }

        // Tìm sản phẩm trong giỏ hàng
        var cartItem = await _cartRepository.GetCartItemAsync(
            cart.CartId,
            productId
        );

        if (cartItem == null)
        {
            throw new KeyNotFoundException(
                "Product not found in cart."
            );
        }

        // Cập nhật số lượng
        cartItem.Quantity = quantity;

        cart.UpdatedAt = DateTime.UtcNow;

        await _cartRepository.UpdateCartItemAsync(cartItem);

        await _cartRepository.SaveChangesAsync();

        return cart;
    }

    // Xóa một sản phẩm khỏi giỏ hàng
    public async Task RemoveCartItemAsync(
        Guid userId,
        Guid productId)
    {
        ValidateUserId(userId);
        ValidateProductId(productId);

        // Tìm giỏ hàng
        var cart = await _cartRepository.GetCartByUserIdAsync(userId);

        if (cart == null)
        {
            throw new KeyNotFoundException(
                "Cart not found."
            );
        }

        // Tìm sản phẩm trong giỏ hàng
        var cartItem = await _cartRepository.GetCartItemAsync(
            cart.CartId,
            productId
        );

        if (cartItem == null)
        {
            throw new KeyNotFoundException(
                "Product not found in cart."
            );
        }

        // Xóa sản phẩm
        await _cartRepository.RemoveCartItemAsync(cartItem);

        cart.UpdatedAt = DateTime.UtcNow;

        await _cartRepository.SaveChangesAsync();
    }

    // Xóa toàn bộ sản phẩm trong giỏ hàng
    public async Task ClearCartAsync(Guid userId)
    {
        ValidateUserId(userId);

        // Tìm giỏ hàng
        var cart = await _cartRepository.GetCartByUserIdAsync(userId);

        if (cart == null)
        {
            return;
        }

        // Xóa tất cả sản phẩm
        await _cartRepository.ClearCartAsync(cart.CartId);

        cart.UpdatedAt = DateTime.UtcNow;

        await _cartRepository.SaveChangesAsync();
    }

    // Lấy giỏ hàng, nếu chưa có thì tạo mới
    private async Task<Cart> GetOrCreateCartAsync(Guid userId)
    {
        var cart = await _cartRepository.GetCartByUserIdAsync(userId);

        if (cart != null)
        {
            return cart;
        }

        cart = new Cart
        {
            CartId = Guid.NewGuid(),
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            CartItems = new List<CartItem>()
        };

        await _cartRepository.CreateCartAsync(cart);

        await _cartRepository.SaveChangesAsync();

        return cart;
    }

    // Kiểm tra UserId
    private static void ValidateUserId(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "UserId is required."
            );
        }
    }

    // Kiểm tra ProductId
    private static void ValidateProductId(Guid productId)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException(
                "ProductId is required."
            );
        }
    }

    // Kiểm tra số lượng
    private static void ValidateQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException(
                "Quantity must be greater than zero."
            );
        }
    }
}