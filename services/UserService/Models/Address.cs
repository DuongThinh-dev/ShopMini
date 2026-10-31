namespace UserService.Models;

public class Address
{
    // Khóa chính
    public Guid AddressId { get; set; }

    // Khóa ngoại liên kết User
    public Guid UserId { get; set; }

    // Thông tin người nhận
    public string ReceiverName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    // Địa chỉ giao hàng
    public string Province { get; set; } = string.Empty;

    public string District { get; set; } = string.Empty;

    public string Ward { get; set; } = string.Empty;

    public string DetailAddress { get; set; } = string.Empty;

    // Địa chỉ mặc định
    public bool IsDefault { get; set; }

    // Quan hệ với User
    public User? User { get; set; }
}