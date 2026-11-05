
using PaymentService.Models;
using PaymentService.Models.Enums;
using PaymentService.Repositories.Interfaces;
using PaymentService.Services.Interfaces;

namespace PaymentService.Services.Implementations;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;

    public PaymentService(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    // 1. Lấy danh sách thanh toán
    public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
    {
        return await _paymentRepository.GetAllAsync();
    }

    // 2. Lấy thanh toán theo ID
    public async Task<Payment?> GetPaymentByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException(
                "PaymentId không hợp lệ."
            );
        }

        return await _paymentRepository.GetByIdAsync(id);
    }

    // 3. Lấy thanh toán theo OrderId
    public async Task<Payment?> GetPaymentByOrderIdAsync(Guid orderId)
    {
        if (orderId == Guid.Empty)
        {
            throw new ArgumentException(
                "OrderId không hợp lệ."
            );
        }

        return await _paymentRepository.GetByOrderIdAsync(orderId);
    }

    // 4. Tạo giao dịch thanh toán
    public async Task<Payment> CreatePaymentAsync(
        Guid orderId,
        decimal amount,
        PaymentMethod paymentMethod)
    {
        // Kiểm tra OrderId
        if (orderId == Guid.Empty)
        {
            throw new ArgumentException(
                "OrderId không hợp lệ."
            );
        }

        // Kiểm tra số tiền
        if (amount <= 0)
        {
            throw new ArgumentException(
                "Số tiền thanh toán phải lớn hơn 0."
            );
        }

        // Kiểm tra phương thức thanh toán
        if (!Enum.IsDefined(paymentMethod))
        {
            throw new ArgumentException(
                "Phương thức thanh toán không hợp lệ."
            );
        }

        // Kiểm tra đơn hàng đã có giao dịch chưa
        var existingPayment =
            await _paymentRepository.GetByOrderIdAsync(orderId);

        if (existingPayment != null)
        {
            throw new InvalidOperationException(
                "Đơn hàng này đã tồn tại giao dịch thanh toán."
            );
        }

        // Khởi tạo giao dịch mới
        var payment = new Payment
        {
            OrderId = orderId,
            Amount = amount,
            PaymentMethod = paymentMethod,
            PaymentStatus = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Thêm giao dịch
        await _paymentRepository.AddAsync(payment);

        // Lưu database
        await _paymentRepository.SaveChangesAsync();

        return payment;
    }

    // 5. Cập nhật trạng thái thanh toán
    public async Task<Payment> UpdatePaymentStatusAsync(
        Guid paymentId,
        PaymentStatus newStatus,
        string? transactionCode = null)
    {
        // Kiểm tra PaymentId
        if (paymentId == Guid.Empty)
        {
            throw new ArgumentException(
                "PaymentId không hợp lệ."
            );
        }

        // Kiểm tra trạng thái mới
        if (!Enum.IsDefined(newStatus))
        {
            throw new ArgumentException(
                "Trạng thái thanh toán không hợp lệ."
            );
        }

        // Tìm giao dịch
        var payment = await _paymentRepository.GetByIdAsync(paymentId);

        if (payment == null)
        {
            throw new KeyNotFoundException(
                "Không tìm thấy giao dịch thanh toán."
            );
        }

        // Chuẩn hóa TransactionCode
        transactionCode = string.IsNullOrWhiteSpace(transactionCode)
            ? null
            : transactionCode.Trim();

        // Kiểm tra độ dài TransactionCode
        if (transactionCode?.Length > 100)
        {
            throw new ArgumentException(
                "TransactionCode không được vượt quá 100 ký tự."
            );
        }

        // Kiểm tra quy tắc chuyển trạng thái
        if (!CanTransition(payment.PaymentStatus, newStatus))
        {
            throw new InvalidOperationException(
                $"Không thể chuyển trạng thái từ " +
                $"{payment.PaymentStatus} sang {newStatus}."
            );
        }

        // Cập nhật trạng thái
        payment.PaymentStatus = newStatus;

        // Cập nhật TransactionCode nếu có
        if (transactionCode != null)
        {
            payment.TransactionCode = transactionCode;
        }

        // Cập nhật thời gian
        payment.UpdatedAt = DateTime.UtcNow;

        // Lưu database
        _paymentRepository.Update(payment);

        await _paymentRepository.SaveChangesAsync();

        return payment;
    }

    // 6. Kiểm tra quy tắc chuyển trạng thái
    private static bool CanTransition(
        PaymentStatus currentStatus,
        PaymentStatus newStatus)
    {
        return currentStatus switch
        {
            PaymentStatus.Pending =>
                newStatus == PaymentStatus.Paid ||
                newStatus == PaymentStatus.Failed,

            PaymentStatus.Paid =>
                newStatus == PaymentStatus.Refunded,

            PaymentStatus.Unpaid => false,

            PaymentStatus.Failed => false,

            PaymentStatus.Refunded => false,

            _ => false
        };
    }
}
