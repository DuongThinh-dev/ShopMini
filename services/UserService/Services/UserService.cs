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
    public async Task<User> CreateUserAsync(User user)
    {
        // Kiểm tra Email đã tồn tại chưa
        var existingUser = await _userRepository
            .GetByEmailAsync(user.Email);

        if (existingUser != null)
        {
            throw new InvalidOperationException(
                "Email đã được sử dụng!"
            );
        }

        // Khởi tạo thông tin User
        user.UserId = Guid.NewGuid();
        user.CreatedAt = DateTime.UtcNow;
        user.IsActive = true;

        // Thêm User vào Database
        await _userRepository.AddAsync(user);

        // Lưu thay đổi
        await _userRepository.SaveChangesAsync();

        return user;
    }

    // Cập nhật User
    public async Task<User?> UpdateUserAsync(Guid id, User user)
    {
        var existingUser = await _userRepository.GetByIdAsync(id);

        if (existingUser == null)
        {
            return null;
        }

        // Cập nhật thông tin
        existingUser.FullName = user.FullName;
        existingUser.Email = user.Email;
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

        await _userRepository.DeleteAsync(user);
        await _userRepository.SaveChangesAsync();

        return true;
    }
}