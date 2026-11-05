
using OrderService.Models;
using OrderService.Repositories.Interfaces;
using OrderService.Services.Interfaces;

namespace OrderService.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    // ==========================================
    // LẤY TẤT CẢ ĐƠN HÀNG
    // ==========================================

    public async Task<List<Order>> GetAllAsync()
    {
        return await _orderRepository.GetAllAsync();
    }

    // ==========================================
    // LẤY ĐƠN HÀNG THEO ID
    // ==========================================

    public async Task<Order?> GetByIdAsync(Guid orderId)
    {
        if (orderId == Guid.Empty)
        {
            throw new ArgumentException("OrderId không hợp lệ.");
        }

        return await _orderRepository.GetByIdAsync(orderId);
    }

    // ==========================================
    // LẤY ĐƠN HÀNG THEO USER ID
    // ==========================================

    public async Task<List<Order>> GetByUserIdAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("UserId không hợp lệ.");
        }

        return await _orderRepository.GetByUserIdAsync(userId);
    }

    // ==========================================
    // TẠO ĐƠN HÀNG
    // ==========================================

    public async Task<Order> CreateAsync(Order order)
    {
        if (order == null)
        {
            throw new ArgumentNullException(nameof(order));
        }

        if (order.UserId == Guid.Empty)
        {
            throw new ArgumentException("UserId không hợp lệ.");
        }

        if (string.IsNullOrWhiteSpace(order.ReceiverName))
        {
            throw new ArgumentException("Tên người nhận không được để trống.");
        }

        if (order.ReceiverName.Length > 100)
        {
            throw new ArgumentException("Tên người nhận không được vượt quá 100 ký tự.");
        }

        if (string.IsNullOrWhiteSpace(order.Phone))
        {
            throw new ArgumentException("Số điện thoại không được để trống.");
        }

        if (order.Phone.Length > 15)
        {
            throw new ArgumentException("Số điện thoại không được vượt quá 15 ký tự.");
        }

        if (string.IsNullOrWhiteSpace(order.ShippingAddress))
        {
            throw new ArgumentException("Địa chỉ giao hàng không được để trống.");
        }

        if (order.ShippingAddress.Length > 500)
        {
            throw new ArgumentException("Địa chỉ giao hàng không được vượt quá 500 ký tự.");
        }

        if (order.OrderItems == null || !order.OrderItems.Any())
        {
            throw new ArgumentException("Đơn hàng phải có ít nhất một sản phẩm.");
        }

        decimal totalAmount = 0;

        foreach (var item in order.OrderItems)
        {
            if (item.ProductId == Guid.Empty)
            {
                throw new ArgumentException("ProductId không hợp lệ.");
            }

            if (string.IsNullOrWhiteSpace(item.ProductName))
            {
                throw new ArgumentException("Tên sản phẩm không được để trống.");
            }

            if (item.ProductName.Length > 200)
            {
                throw new ArgumentException("Tên sản phẩm không được vượt quá 200 ký tự.");
            }

            if (item.UnitPrice < 0)
            {
                throw new ArgumentException("Đơn giá không được âm.");
            }

            if (item.Quantity <= 0)
            {
                throw new ArgumentException("Số lượng sản phẩm phải lớn hơn 0.");
            }

            // item.OrderItemId = Guid.NewGuid();
            // item.Subtotal = item.UnitPrice * item.Quantity;

            // totalAmount += item.Subtotal;

            item.OrderItemId = Guid.NewGuid();

            // Tính tổng tiền đơn hàng
            decimal subtotal = item.UnitPrice * item.Quantity;

            totalAmount += subtotal;
        }

        // Thiết lập thông tin đơn hàng
        order.OrderId = Guid.NewGuid();
        order.CreatedAt = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;
        order.Status = OrderStatus.PendingConfirmation;

        // Chờ thanh toán
        order.Status = OrderStatus.PendingPayment;

        // Gán OrderId cho OrderItems
        foreach (var item in order.OrderItems)
        {
            item.OrderId = order.OrderId;
        }

        await _orderRepository.AddAsync(order);
        await _orderRepository.SaveChangesAsync();

        return order;
    }

    // ==========================================
    // CẬP NHẬT THÔNG TIN ĐƠN HÀNG
    // ==========================================

    public async Task<Order?> UpdateAsync(Order order)
    {
        if (order == null)
        {
            throw new ArgumentNullException(nameof(order));
        }

        if (order.OrderId == Guid.Empty)
        {
            throw new ArgumentException("OrderId không hợp lệ.");
        }

        if (string.IsNullOrWhiteSpace(order.ReceiverName))
        {
            throw new ArgumentException("Tên người nhận không được để trống.");
        }

        if (order.ReceiverName.Length > 100)
        {
            throw new ArgumentException("Tên người nhận không được vượt quá 100 ký tự.");
        }

        if (string.IsNullOrWhiteSpace(order.Phone))
        {
            throw new ArgumentException("Số điện thoại không được để trống.");
        }

        if (order.Phone.Length > 15)
        {
            throw new ArgumentException("Số điện thoại không được vượt quá 15 ký tự.");
        }

        if (string.IsNullOrWhiteSpace(order.ShippingAddress))
        {
            throw new ArgumentException("Địa chỉ giao hàng không được để trống.");
        }

        if (order.ShippingAddress.Length > 500)
        {
            throw new ArgumentException("Địa chỉ giao hàng không được vượt quá 500 ký tự.");
        }

        var existingOrder = await _orderRepository.GetByIdAsync(order.OrderId);

        if (existingOrder == null)
        {
            return null;
        }

        // Chỉ cập nhật khi đơn hàng đang chờ xác nhận
        if (existingOrder.Status != OrderStatus.PendingConfirmation)
        {
            throw new InvalidOperationException(
                "Chỉ có thể cập nhật đơn hàng đang chờ xác nhận."
            );
        }

        existingOrder.ReceiverName = order.ReceiverName;
        existingOrder.Phone = order.Phone;
        existingOrder.ShippingAddress = order.ShippingAddress;
        existingOrder.UpdatedAt = DateTime.UtcNow;

        await _orderRepository.UpdateAsync(existingOrder);
        await _orderRepository.SaveChangesAsync();

        return existingOrder;
    }

    // ==========================================
    // CẬP NHẬT TRẠNG THÁI ĐƠN HÀNG
    // ==========================================

    public async Task<Order?> UpdateStatusAsync(
        Guid orderId,
        OrderStatus status)
    {
        if (orderId == Guid.Empty)
        {
            throw new ArgumentException("OrderId không hợp lệ.");
        }

        if (!Enum.IsDefined(typeof(OrderStatus), status))
        {
            throw new ArgumentException("Trạng thái đơn hàng không hợp lệ.");
        }

        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order == null)
        {
            return null;
        }

        bool isValidTransition = (order.Status, status) switch
        {
            (OrderStatus.PendingPayment, OrderStatus.PaymentFailed) => true,

            (OrderStatus.PendingPayment, OrderStatus.PendingConfirmation) => true,

            (OrderStatus.PendingConfirmation, OrderStatus.Confirmed) => true,

            (OrderStatus.Confirmed, OrderStatus.Shipping) => true,

            (OrderStatus.Shipping, OrderStatus.Completed) => true,

            (OrderStatus.PendingConfirmation, OrderStatus.Cancelled) => true,

            (OrderStatus.Confirmed, OrderStatus.Cancelled) => true,

            _ => false
        };

        if (!isValidTransition)
        {
            throw new InvalidOperationException(
                $"Không thể chuyển trạng thái từ {order.Status} sang {status}."
            );
        }

        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;

        await _orderRepository.UpdateAsync(order);
        await _orderRepository.SaveChangesAsync();

        return order;
    }

    // ==========================================
    // HỦY ĐƠN HÀNG
    // ==========================================

    public async Task<Order?> CancelAsync(Guid orderId, Guid userId)
    {
        if (orderId == Guid.Empty || userId == Guid.Empty)
        {
            throw new ArgumentException("OrderId hoặc UserId không hợp lệ.");
        }

        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order == null || order.UserId != userId)
        {
            return null;
        }

        if (order.Status != OrderStatus.PendingPayment &&
            order.Status != OrderStatus.PendingConfirmation &&
            order.Status != OrderStatus.Confirmed)
        {
            throw new InvalidOperationException(
                "Không thể hủy đơn hàng ở trạng thái hiện tại."
            );
        }

        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = DateTime.UtcNow;

        await _orderRepository.UpdateAsync(order);
        await _orderRepository.SaveChangesAsync();

        return order;
    }
}
