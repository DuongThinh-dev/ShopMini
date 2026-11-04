
using UserService.Models;
using UserService.Repositories.Interfaces;
using UserService.Services.Interfaces;

namespace UserService.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;

    public RoleService(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    // Lấy danh sách tất cả Role
    public async Task<IEnumerable<Role>> GetAllRolesAsync()
    {
        return await _roleRepository.GetAllAsync();
    }

    // Lấy Role theo ID
    public async Task<Role?> GetRoleByIdAsync(Guid id)
    {
        return await _roleRepository.GetByIdAsync(id);
    }
}
