using UserService.Models;

namespace UserService.Repositories.Interfaces;

public interface IUserRepository
{
    // Lấy danh sách tất cả User
    Task<IEnumerable<User>> GetAllAsync();

    // Tìm User theo ID
    Task<User?> GetByIdAsync(Guid id);

    // Tìm User theo Email
    Task<User?> GetByEmailAsync(string email);

    // Thêm User mới
    Task AddAsync(User user);

    // Cập nhật User
    Task UpdateAsync(User user);

    // Xóa User
    Task DeleteAsync(User user);

    // Lưu thay đổi vào Database
    Task SaveChangesAsync();
}