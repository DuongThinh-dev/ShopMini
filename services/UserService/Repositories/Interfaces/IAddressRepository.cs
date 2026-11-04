
using UserService.Models;

namespace UserService.Repositories.Interfaces;

public interface IAddressRepository
{
    // Lấy tất cả địa chỉ của một User
    Task<IEnumerable<Address>> GetByUserIdAsync(Guid userId);

    // Lấy địa chỉ theo ID
    Task<Address?> GetByIdAsync(Guid id);

    // Thêm địa chỉ
    Task AddAsync(Address address);

    // Cập nhật địa chỉ
    Task UpdateAsync(Address address);

    // Xóa địa chỉ
    Task DeleteAsync(Address address);

    // Lưu thay đổi
    Task SaveChangesAsync();
}
