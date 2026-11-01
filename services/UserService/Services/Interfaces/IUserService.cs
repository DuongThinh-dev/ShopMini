using UserService.Models;

namespace UserService.Services.Interfaces;

public interface IUserService
{
    // Lấy danh sách tất cả User
    Task<IEnumerable<User>> GetAllUsersAsync();

    // Lấy User theo ID
    Task<User?> GetUserByIdAsync(Guid id);

    // Lấy User theo Email
    Task<User?> GetUserByEmailAsync(string email);

    // Tạo User mới
    Task<User> CreateUserAsync(User user);

    // Cập nhật User
    Task<User?> UpdateUserAsync(Guid id, User user);

    // Xóa User
    Task<bool> DeleteUserAsync(Guid id);
}