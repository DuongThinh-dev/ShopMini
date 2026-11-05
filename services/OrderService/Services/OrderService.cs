
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
    // LẤY DANH SÁCH ĐƠN HÀNG THEO USER ID
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

        if (string.IsNullOrWhiteSpace(order.ShippingAddress))
        {
            throw new ArgumentException("Địa chỉ giao hàng không được để trống.");
        }

        if (order.ShippingAddress.Length > 500)
        {
            throw new ArgumentException("Địa chỉ giao hàng không được vượt quá 500 ký tự.");
        }

        if (order.Note?.Length > 500)
        {
            throw new ArgumentException("Ghi chú không được vượt quá 500 ký tự.");
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

            // Tính thành tiền của từng sản phẩm
            item.OrderItemId = Guid.NewGuid();
            item.Subtotal = item.UnitPrice * item.Quantity;

            totalAmount += item.Subtotal;
        }

        // Thiết lập thông tin đơn hàng
        order.OrderId = Guid.NewGuid();
        order.OrderDate = DateTime.UtcNow;
        order.TotalAmount = totalAmount;
        order.Status = OrderStatus.Pending;

        // Gán OrderId cho các OrderItem
        foreach (var item in order.OrderItems)
        {
            item.OrderId = order.OrderId;
        }

        // Thêm đơn hàng vào database
        await _orderRepository.AddAsync(order);

        // Lưu thay đổi
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

        if (string.IsNullOrWhiteSpace(order.ShippingAddress))
        {
            throw new ArgumentException("Địa chỉ giao hàng không được để trống.");
        }

        if (order.ShippingAddress.Length > 500)
        {
            throw new ArgumentException("Địa chỉ giao hàng không được vượt quá 500 ký tự.");
        }

        if (order.Note?.Length > 500)
        {
            throw new ArgumentException("Ghi chú không được vượt quá 500 ký tự.");
        }

        var existingOrder = await _orderRepository.GetByIdAsync(order.OrderId);

        if (existingOrder == null)
        {
            return null;
        }

        // Chỉ cho phép cập nhật khi đơn hàng đang chờ xác nhận
        if (existingOrder.Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException(
                "Chỉ có thể cập nhật đơn hàng đang chờ xác nhận."
            );
        }

        // Chỉ cập nhật thông tin giao hàng và ghi chú
        existingOrder.ShippingAddress = order.ShippingAddress;
        existingOrder.Note = order.Note;

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
            (OrderStatus.Pending, OrderStatus.Confirmed) => true,

            (OrderStatus.Confirmed, OrderStatus.Processing) => true,

            (OrderStatus.Processing, OrderStatus.Shipped) => true,

            (OrderStatus.Shipped, OrderStatus.Delivered) => true,

            (OrderStatus.Pending, OrderStatus.Cancelled) => true,

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

        if (order.Status != OrderStatus.Pending &&
            order.Status != OrderStatus.Confirmed)
        {
            throw new InvalidOperationException(
                "Không thể hủy đơn hàng ở trạng thái hiện tại."
            );
        }

        order.Status = OrderStatus.Cancelled;

        await _orderRepository.UpdateAsync(order);
        await _orderRepository.SaveChangesAsync();

        return order;
    }
}
