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

        var response = orders.Select(MapToResponse).ToList();

        return Ok(response);
    }

    // GET: api/orders/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderResponse>> GetById(Guid id)
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

    // GET: api/orders/user/{userId}
    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<List<OrderResponse>>> GetByUserId(Guid userId)
    {
        try
        {
            var orders = await _orderService.GetByUserIdAsync(userId);

            var response = orders.Select(MapToResponse).ToList();

            return Ok(response);
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
            // 1. Ánh xạ Request DTO -> Entity
            var order = new Order
            {
                UserId = request.UserId,
                ShippingAddress = request.ShippingAddress,
                Note = request.Note,

                OrderItems = request.OrderItems.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity
                }).ToList()
            };

            // 2. Gọi Service để xử lý nghiệp vụ và lưu dữ liệu
            var createdOrder = await _orderService.CreateAsync(order);

            // 3. Ánh xạ Entity -> Response DTO
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
                ShippingAddress = request.ShippingAddress,
                Note = request.Note
            };

            var updatedOrder = await _orderService.UpdateAsync(order);

            if (updatedOrder == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy đơn hàng hoặc đơn hàng không thể cập nhật."
                });
            }

            // Entity -> Response DTO
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
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            ShippingAddress = order.ShippingAddress,
            Note = order.Note,

            OrderItems = order.OrderItems.Select(item => new OrderItemResponse
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