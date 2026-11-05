
using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using PaymentService.Models;
using PaymentService.Repositories.Interfaces;

namespace PaymentService.Repositories.Implementations;

public class PaymentRepository : IPaymentRepository
{
    private readonly PaymentDbContext _context;

    public PaymentRepository(PaymentDbContext context)
    {
        _context = context;
    }

    // Lấy tất cả thanh toán
    public async Task<IEnumerable<Payment>> GetAllAsync()
    {
        return await _context.Payments
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    // Lấy thanh toán theo ID
    public async Task<Payment?> GetByIdAsync(Guid id)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(p => p.PaymentId == id);
    }

    // Lấy thanh toán theo OrderId
    public async Task<Payment?> GetByOrderIdAsync(Guid orderId)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(p => p.OrderId == orderId);
    }

    // Thêm thanh toán mới
    public async Task AddAsync(Payment payment)
    {
        await _context.Payments.AddAsync(payment);
    }

    // Cập nhật thanh toán
    public void Update(Payment payment)
    {
        _context.Payments.Update(payment);
    }

    // Lưu thay đổi vào database
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
