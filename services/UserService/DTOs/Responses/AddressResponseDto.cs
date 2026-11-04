
namespace UserService.DTOs.Responses;

public class AddressResponseDto
{
    public Guid AddressId { get; set; }

    public Guid UserId { get; set; }

    public string ReceiverName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Province { get; set; } = string.Empty;

    public string District { get; set; } = string.Empty;

    public string Ward { get; set; } = string.Empty;

    public string DetailAddress { get; set; } = string.Empty;

    public bool IsDefault { get; set; }
}
