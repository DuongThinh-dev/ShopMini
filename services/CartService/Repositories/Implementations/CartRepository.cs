using CartService.Data;
using CartService.Models;
using CartService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CartService.Repositories.Implementations;

public class CartRepository : ICartRepository
{
    private readonly CartDbContext _context;

    public CartRepository(CartDbContext context)
    {
        _context = context;
    }

    // Lấy giỏ hàng theo UserId
    public async Task<Cart?> GetCartByUserIdAsync(Guid userId)
    {
        return await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    // Lấy giỏ hàng theo CartId
    public async Task<Cart?> GetCartByIdAsync(Guid cartId)
    {
        return await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.CartId == cartId);
    }

    // Tìm sản phẩm trong giỏ hàng
    public async Task<CartItem?> GetCartItemAsync(
        Guid cartId,
        Guid productId)
    {
        return await _context.CartItems
            .FirstOrDefaultAsync(ci =>
                ci.CartId == cartId &&
                ci.ProductId == productId);
    }

    // Tạo giỏ hàng mới
    public async Task CreateCartAsync(Cart cart)
    {
        await _context.Carts.AddAsync(cart);
    }

    // Thêm sản phẩm vào giỏ hàng
    public async Task AddCartItemAsync(CartItem cartItem)
    {
        await _context.CartItems.AddAsync(cartItem);
    }

    // Cập nhật sản phẩm trong giỏ hàng
    public Task UpdateCartItemAsync(CartItem cartItem)
    {
        _context.CartItems.Update(cartItem);

        return Task.CompletedTask;
    }

    // Xóa một sản phẩm khỏi giỏ hàng
    public Task RemoveCartItemAsync(CartItem cartItem)
    {
        _context.CartItems.Remove(cartItem);

        return Task.CompletedTask;
    }

    // Xóa toàn bộ sản phẩm trong giỏ hàng
    public async Task ClearCartAsync(Guid cartId)
    {
        var cartItems = await _context.CartItems
            .Where(ci => ci.CartId == cartId)
            .ToListAsync();

        _context.CartItems.RemoveRange(cartItems);
    }

    // Lưu thay đổi vào Database
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}