
using Microsoft.AspNetCore.Mvc;
using PaymentService.DTOs.Payment;
using PaymentService.Services.Interfaces;

namespace PaymentService.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    // GET: api/payments
    // Lấy danh sách tất cả giao dịch thanh toán
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentResponse>>> GetAllPayments()
    {
        var payments = await _paymentService.GetAllPaymentsAsync();

        // Chuyển Entity sang Response DTO
        var response = payments
            .Select(PaymentResponse.FromEntity)
            .ToList();

        return Ok(response);
    }

    // GET: api/payments/{id}
    // Lấy giao dịch thanh toán theo PaymentId
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PaymentResponse>> GetPaymentById(Guid id)
    {
        var payment = await _paymentService.GetPaymentByIdAsync(id);

        if (payment == null)
        {
            return NotFound(new
            {
                message = "Không tìm thấy giao dịch thanh toán."
            });
        }

        // Chuyển Entity sang Response DTO
        var response = PaymentResponse.FromEntity(payment);

        return Ok(response);
    }

    // GET: api/payments/by-order/{orderId}
    // Lấy giao dịch thanh toán theo OrderId
    [HttpGet("by-order/{orderId:guid}")]
    public async Task<ActionResult<PaymentResponse>> GetPaymentByOrderId(Guid orderId)
    {
        var payment = await _paymentService.GetPaymentByOrderIdAsync(orderId);

        if (payment == null)
        {
            return NotFound(new
            {
                message = "Không tìm thấy giao dịch thanh toán cho đơn hàng này."
            });
        }

        // Chuyển Entity sang Response DTO
        var response = PaymentResponse.FromEntity(payment);

        return Ok(response);
    }

    // POST: api/payments
    // Tạo giao dịch thanh toán mới
    [HttpPost]
    public async Task<ActionResult<PaymentResponse>> CreatePayment(
        [FromBody] CreatePaymentRequest request)
    {
        try
        {
            // Request DTO -> Service -> Entity
            var payment = await _paymentService.CreatePaymentAsync(
                request.OrderId,
                request.Amount,
                request.PaymentMethod
            );

            // Entity -> Response DTO
            var response = PaymentResponse.FromEntity(payment);

            return CreatedAtAction(
                nameof(GetPaymentById),
                new { id = payment.PaymentId },
                response
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            // Ví dụ: đơn hàng đã có giao dịch thanh toán
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    // PUT: api/payments/{id}/status
    // Cập nhật trạng thái giao dịch thanh toán
    [HttpPut("{id:guid}/status")]
    public async Task<ActionResult<PaymentResponse>> UpdatePaymentStatus(
        Guid id,
        [FromBody] UpdatePaymentStatusRequest request)
    {
        try
        {
            // Request DTO -> Service -> Entity
            var payment = await _paymentService.UpdatePaymentStatusAsync(
                id,
                request.PaymentStatus,
                request.TransactionCode,
                request.FailureReason
            );

            // Entity -> Response DTO
            var response = PaymentResponse.FromEntity(payment);

            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            // Ví dụ: không cho phép chuyển từ Failed sang Completed
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }
}
