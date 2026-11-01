using Microsoft.AspNetCore.Mvc;
using UserService.DTOs.Requests;
using UserService.DTOs.Responses;
using UserService.Services.Interfaces;

namespace UserService.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    // Chuyển Model User thành UserResponseDto
    private static UserResponseDto MapToDto(Models.User user)
    {
        return new UserResponseDto
        {
            UserId = user.UserId,
            FullName = user.FullName,
            Email = user.Email,
            RoleId = user.RoleId,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    // GET: api/users
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();

        var result = users.Select(MapToDto);

        return Ok(result);
    }

    // GET: api/users/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
        {
            return NotFound("Không tìm thấy người dùng!");
        }

        return Ok(MapToDto(user));
    }

    // GET: api/users/by-email?email=example@gmail.com
    [HttpGet("by-email")]
    public async Task<IActionResult> GetUserByEmail(
        [FromQuery] string email)
    {
        var user = await _userService.GetUserByEmailAsync(email);

        if (user == null)
        {
            return NotFound("Không tìm thấy người dùng!");
        }

        return Ok(MapToDto(user));
    }

    // POST: api/users
    [HttpPost]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserRequestDto request)
    {
        try
        {
            var user = await _userService.CreateUserAsync(request);

            var result = MapToDto(user);

            return CreatedAtAction(
                nameof(GetUserById),
                new { id = user.UserId },
                result
            );
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // PUT: api/users/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(
        Guid id,
        [FromBody] UpdateUserRequestDto request)
    {
        try
        {
            var user = await _userService.UpdateUserAsync(id, request);

            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng!");
            }

            return Ok(MapToDto(user));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // DELETE: api/users/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        bool deleted = await _userService.DeleteUserAsync(id);

        if (!deleted)
        {
            return NotFound("Không tìm thấy người dùng!");
        }

        return NoContent();
    }
}