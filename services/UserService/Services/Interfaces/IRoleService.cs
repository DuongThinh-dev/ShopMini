
using UserService.Models;

namespace UserService.Services.Interfaces;

public interface IRoleService
{
    // Lấy danh sách tất cả Role
    Task<IEnumerable<Role>> GetAllRolesAsync();

    // Lấy Role theo ID
    Task<Role?> GetRoleByIdAsync(Guid id);
}
