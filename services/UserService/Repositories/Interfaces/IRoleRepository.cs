
using UserService.Models;

namespace UserService.Repositories.Interfaces;

public interface IRoleRepository
{
    // Lấy danh sách tất cả Role
    Task<IEnumerable<Role>> GetAllAsync();

    // Tìm Role theo ID
    Task<Role?> GetByIdAsync(Guid id);

    // Thêm Role mới
    Task AddAsync(Role role);

    // Cập nhật Role
    Task UpdateAsync(Role role);

    // Xóa Role
    Task DeleteAsync(Role role);

    // Lưu thay đổi vào Database
    Task SaveChangesAsync();
}
