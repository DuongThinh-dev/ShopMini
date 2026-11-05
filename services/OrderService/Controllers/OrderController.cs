
using Microsoft.AspNetCore.Mvc;
using OrderService.DTOs.Order;
using OrderService.Models;
using OrderService.Services.Interfaces;

namespace OrderService.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // GET: api/orders
    [HttpGet]
    public async Task<ActionResult<List<OrderResponse>>> GetAll()
    {
        var orders = await _orderService.GetAllAsync();

        return Ok(orders.Select(MapToResponse).ToList());
    }

    // GET: api/orders/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderResponse>> GetById(Guid id)
    {
        try
        {
            var order = await _orderService.GetByIdAsync(id);

            if (order == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy đơn hàng."
                });
            }

            return Ok(MapToResponse(order));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: api/orders/user/{userId}
    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<List<OrderResponse>>> GetByUserId(
        Guid userId)
    {
        try
        {
            var orders = await _orderService.GetByUserIdAsync(userId);

            return Ok(orders.Select(MapToResponse).ToList());
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST: api/orders
    [HttpPost]
    public async Task<ActionResult<OrderResponse>> Create(
        [FromBody] CreateOrderRequest request)
    {
        try
        {
            // Request DTO -> Entity
            var order = new Order
            {
                UserId = request.UserId,
                ReceiverName = request.ReceiverName,
                Phone = request.Phone,
                ShippingAddress = request.ShippingAddress,

                OrderItems = request.OrderItems.Select(item =>
                    new OrderItem
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity
                    }).ToList()
            };

            var createdOrder = await _orderService.CreateAsync(order);

            // Entity -> Response DTO
            var response = MapToResponse(createdOrder);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdOrder.OrderId },
                response
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT: api/orders/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<OrderResponse>> Update(
        Guid id,
        [FromBody] UpdateOrderRequest request)
    {
        try
        {
            // Request DTO -> Entity
            var order = new Order
            {
                OrderId = id,
                ReceiverName = request.ReceiverName,
                Phone = request.Phone,
                ShippingAddress = request.ShippingAddress
            };

            var updatedOrder = await _orderService.UpdateAsync(order);

            if (updatedOrder == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy đơn hàng."
                });
            }

            return Ok(MapToResponse(updatedOrder));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PATCH: api/orders/{id}/status
    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<OrderResponse>> UpdateStatus(
        Guid id,
        [FromBody] UpdateOrderStatusRequest request)
    {
        try
        {
            var updatedOrder = await _orderService.UpdateStatusAsync(
                id,
                request.Status
            );

            if (updatedOrder == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy đơn hàng."
                });
            }

            return Ok(MapToResponse(updatedOrder));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PATCH: api/orders/{id}/cancel?userId={userId}
    [HttpPatch("{id:guid}/cancel")]
    public async Task<ActionResult<OrderResponse>> Cancel(
        Guid id,
        [FromQuery] Guid userId)
    {
        try
        {
            var cancelledOrder = await _orderService.CancelAsync(id, userId);

            if (cancelledOrder == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy đơn hàng hoặc bạn không có quyền hủy đơn hàng này."
                });
            }

            return Ok(MapToResponse(cancelledOrder));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Entity -> Response DTO
    private static OrderResponse MapToResponse(Order order)
    {
        return new OrderResponse
        {
            OrderId = order.OrderId,
            UserId = order.UserId,
            TotalAmount = order.TotalAmount,

            ReceiverName = order.ReceiverName,
            Phone = order.Phone,
            ShippingAddress = order.ShippingAddress,

            Status = order.Status,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,

            OrderItems = order.OrderItems.Select(item =>
                new OrderItemResponse
                {
                    OrderItemId = item.OrderItemId,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    Subtotal = item.Subtotal
                }).ToList()
        };
    }
}
