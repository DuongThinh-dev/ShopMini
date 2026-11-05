namespace OrderService.Models;

public enum OrderStatus
{
    PendingPayment,
    PaymentFailed,
    PendingConfirmation,
    Confirmed,
    Shipping,
    Completed,
    Cancelled
}