using UserService.DTOs.Requests;
using UserService.Models;
using UserService.Repositories.Interfaces;
using UserService.Services.Interfaces;

namespace UserService.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // Lấy danh sách tất cả User
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    // Lấy User theo ID
    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    // Lấy User theo Email
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }

    // Tạo User mới
    public async Task<User> CreateUserAsync(CreateUserRequestDto request)
    {
        string email = request.Email.Trim().ToLowerInvariant();

        // Kiểm tra Email đã tồn tại
        var existingUser = await _userRepository.GetByEmailAsync(email);

        if (existingUser != null)
        {
            throw new InvalidOperationException("Email đã được sử dụng!");
        }

        // Khởi tạo User
        var user = new User
        {
            UserId = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            Email = email,

            // Băm mật khẩu trước khi lưu
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),

            RoleId = request.RoleId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return user;
    }

    // Cập nhật User
    public async Task<User?> UpdateUserAsync(Guid id, UpdateUserRequestDto request)
    {
        var existingUser = await _userRepository.GetByIdAsync(id);

        if (existingUser == null)
        {
            return null;
        }

        string email = request.Email.Trim().ToLowerInvariant();

        // Kiểm tra Email có thuộc tài khoản khác không
        var emailUser = await _userRepository.GetByEmailAsync(email);

        if (emailUser != null && emailUser.UserId != id)
        {
            throw new InvalidOperationException("Email đã được sử dụng!");
        }

        existingUser.FullName = request.FullName.Trim();
        existingUser.Email = email;
        existingUser.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(existingUser);
        await _userRepository.SaveChangesAsync();

        return existingUser;
    }

    // Xóa User
    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
        {
            return false;
        }

        // Xóa mềm: vô hiệu hóa tài khoản
        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return true;
    }
}