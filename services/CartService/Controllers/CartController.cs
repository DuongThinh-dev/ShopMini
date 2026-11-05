using CartService.DTOs.Cart;
using CartService.Models;
using CartService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CartService.Controllers;

[ApiController]
[Route("api/carts")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    // GET: api/carts/{userId}
    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<CartResponse>> GetCart(Guid userId)
    {
        try
        {
            var cart = await _cartService.GetCartAsync(userId);

            var response = MapToCartResponse(cart);

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST: api/carts/{userId}/items
    [HttpPost("{userId:guid}/items")]
    public async Task<ActionResult<CartResponse>> AddToCart(
        Guid userId,
        [FromBody] AddToCartRequest request)
    {
        try
        {
            var cart = await _cartService.AddToCartAsync(
                userId,
                request.ProductId,
                request.Quantity
            );

            var response = MapToCartResponse(cart);

            return CreatedAtAction(
                nameof(GetCart),
                new { userId },
                response
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT: api/carts/{userId}/items/{productId}
    [HttpPut("{userId:guid}/items/{productId:guid}")]
    public async Task<ActionResult<CartResponse>> UpdateCartItem(
        Guid userId,
        Guid productId,
        [FromBody] UpdateCartItemRequest request)
    {
        try
        {
            var cart = await _cartService.UpdateCartItemAsync(
                userId,
                productId,
                request.Quantity
            );

            var response = MapToCartResponse(cart);

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // DELETE: api/carts/{userId}/items/{productId}
    [HttpDelete("{userId:guid}/items/{productId:guid}")]
    public async Task<IActionResult> RemoveCartItem(
        Guid userId,
        Guid productId)
    {
        try
        {
            await _cartService.RemoveCartItemAsync(userId, productId);

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // DELETE: api/carts/{userId}
    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> ClearCart(Guid userId)
    {
        try
        {
            await _cartService.ClearCartAsync(userId);

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Chuyển Cart Entity thành CartResponse DTO
    private static CartResponse MapToCartResponse(Cart cart)
    {
        return new CartResponse
        {
            CartId = cart.CartId,
            UserId = cart.UserId,
            CreatedAt = cart.CreatedAt,
            UpdatedAt = cart.UpdatedAt,

            Items = cart.CartItems.Select(item => new CartItemResponse
            {
                CartItemId = item.CartItemId,
                ProductId = item.ProductId,
                Quantity = item.Quantity
                // AddedAt = item.AddedAt
            }).ToList()
        };
    }
}