
using UserService.DTOs.Requests;
using UserService.Models;

namespace UserService.Services.Interfaces;

public interface IAddressService
{
    // Lấy danh sách địa chỉ của User
    Task<IEnumerable<Address>> GetAddressesByUserIdAsync(Guid userId);

    // Lấy địa chỉ theo ID
    Task<Address?> GetAddressByIdAsync(Guid id);

    // Thêm địa chỉ
    Task<Address> CreateAddressAsync(CreateAddressRequestDto request);

    // Cập nhật địa chỉ
    Task<Address?> UpdateAddressAsync(
        Guid id,
        UpdateAddressRequestDto request);

    // Đặt địa chỉ làm mặc định
    Task<bool> SetDefaultAddressAsync(Guid id);

    // Xóa địa chỉ
    Task<bool> DeleteAddressAsync(Guid id);
}
