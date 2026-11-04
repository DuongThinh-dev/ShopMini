
using UserService.DTOs.Requests;
using UserService.Models;
using UserService.Repositories.Interfaces;
using UserService.Services.Interfaces;

namespace UserService.Services;

public class AddressService : IAddressService
{
    private readonly IAddressRepository _addressRepository;
    private readonly IUserRepository _userRepository;

    public AddressService(
        IAddressRepository addressRepository,
        IUserRepository userRepository)
    {
        _addressRepository = addressRepository;
        _userRepository = userRepository;
    }

    // Lấy danh sách địa chỉ của User
    public async Task<IEnumerable<Address>> GetAddressesByUserIdAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            throw new InvalidOperationException("User không tồn tại!");
        }

        return await _addressRepository.GetByUserIdAsync(userId);
    }

    // Lấy địa chỉ theo ID
    public async Task<Address?> GetAddressByIdAsync(Guid id)
    {
        return await _addressRepository.GetByIdAsync(id);
    }

    // Thêm địa chỉ
    public async Task<Address> CreateAddressAsync(
        CreateAddressRequestDto request)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);

        if (user == null)
        {
            throw new InvalidOperationException("User không tồn tại!");
        }

        var addresses = (await _addressRepository
            .GetByUserIdAsync(request.UserId)).ToList();

        // Nếu User chưa có địa chỉ thì địa chỉ đầu tiên là mặc định.
        // Nếu Client chọn IsDefault = true thì cũng đặt làm mặc định.
        bool isDefault = addresses.Count == 0 || request.IsDefault;

        // Nếu địa chỉ mới là mặc định, bỏ mặc định của địa chỉ cũ.
        if (isDefault)
        {
            foreach (var address in addresses)
            {
                address.IsDefault = false;

                await _addressRepository.UpdateAsync(address);
            }
        }

        var newAddress = new Address
        {
            AddressId = Guid.NewGuid(),
            UserId = request.UserId,
            ReceiverName = request.ReceiverName.Trim(),
            Phone = request.Phone.Trim(),
            Province = request.Province.Trim(),
            District = request.District.Trim(),
            Ward = request.Ward.Trim(),
            DetailAddress = request.DetailAddress.Trim(),
            IsDefault = isDefault
        };

        await _addressRepository.AddAsync(newAddress);
        await _addressRepository.SaveChangesAsync();

        return newAddress;
    }

    // Cập nhật địa chỉ
    public async Task<Address?> UpdateAddressAsync(
        Guid id,
        UpdateAddressRequestDto request)
    {
        var address = await _addressRepository.GetByIdAsync(id);

        if (address == null)
        {
            return null;
        }

        // Nếu người dùng muốn đặt địa chỉ này làm mặc định
        if (request.IsDefault)
        {
            await SetDefaultAddressAsync(id);

            // Lấy lại địa chỉ sau khi thay đổi trạng thái mặc định
            address = await _addressRepository.GetByIdAsync(id);

            if (address == null)
            {
                return null;
            }
        }

        address.ReceiverName = request.ReceiverName.Trim();
        address.Phone = request.Phone.Trim();
        address.Province = request.Province.Trim();
        address.District = request.District.Trim();
        address.Ward = request.Ward.Trim();
        address.DetailAddress = request.DetailAddress.Trim();

        // Nếu request.IsDefault = false thì giữ nguyên trạng thái
        // mặc định hiện tại của địa chỉ.
        await _addressRepository.UpdateAsync(address);
        await _addressRepository.SaveChangesAsync();

        return address;
    }

    // Đặt địa chỉ làm mặc định
    public async Task<bool> SetDefaultAddressAsync(Guid id)
    {
        var address = await _addressRepository.GetByIdAsync(id);

        if (address == null)
        {
            return false;
        }

        var addresses = (await _addressRepository
            .GetByUserIdAsync(address.UserId)).ToList();

        foreach (var item in addresses)
        {
            item.IsDefault = item.AddressId == id;

            await _addressRepository.UpdateAsync(item);
        }

        await _addressRepository.SaveChangesAsync();

        return true;
    }

    // Xóa địa chỉ
    public async Task<bool> DeleteAddressAsync(Guid id)
    {
        var address = await _addressRepository.GetByIdAsync(id);

        if (address == null)
        {
            return false;
        }

        var addresses = (await _addressRepository
            .GetByUserIdAsync(address.UserId)).ToList();

        await _addressRepository.DeleteAsync(address);

        // Nếu xóa địa chỉ mặc định và User vẫn còn địa chỉ khác,
        // chọn địa chỉ đầu tiên còn lại làm mặc định.
        if (address.IsDefault)
        {
            var nextDefault = addresses
                .FirstOrDefault(a => a.AddressId != id);

            if (nextDefault != null)
            {
                nextDefault.IsDefault = true;

                await _addressRepository.UpdateAsync(nextDefault);
            }
        }

        await _addressRepository.SaveChangesAsync();

        return true;
    }
}
