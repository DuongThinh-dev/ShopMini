
using Microsoft.AspNetCore.Mvc;
using UserService.Services.Interfaces;

namespace UserService.Controllers;

[ApiController]
[Route("api/roles")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    // GET: api/roles
    [HttpGet]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _roleService.GetAllRolesAsync();

        return Ok(roles);
    }

    // GET: api/roles/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetRoleById(Guid id)
    {
        var role = await _roleService.GetRoleByIdAsync(id);

        if (role == null)
        {
            return NotFound(new
            {
                message = "Role not found."
            });
        }

        return Ok(role);
    }
}
