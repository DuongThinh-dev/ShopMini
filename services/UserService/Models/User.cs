namespace UserService.Models;

public class User
{
    // Khóa chính
    public Guid UserId { get; set; }

    // Thông tin tài khoản
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    // Khóa ngoại liên kết Role
    public Guid RoleId { get; set; }

    // Trạng thái tài khoản
    public bool IsActive { get; set; }

    // Thời gian tạo và cập nhật
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    // Quan hệ với Role
    public Role? Role { get; set; }

    // Một User có nhiều Address
    public ICollection<Address> Addresses { get; set; }
        = new List<Address>();
}