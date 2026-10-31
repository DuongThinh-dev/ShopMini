namespace UserService.Models;

public class Role
{
    // Khóa chính
    public Guid RoleId { get; set; }

    // Tên vai trò
    public string RoleName { get; set; } = string.Empty;

    // Một Role có nhiều User
    public ICollection<User> Users { get; set; }
        = new List<User>();
}