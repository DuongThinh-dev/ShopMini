
using System.ComponentModel.DataAnnotations;

namespace UserService.DTOs.Requests;

public class CreateAddressRequestDto
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    [StringLength(120)]
    public string ReceiverName { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Province { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string District { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Ward { get; set; } = string.Empty;

    [Required]
    [StringLength(300)]
    public string DetailAddress { get; set; } = string.Empty;

    public bool IsDefault { get; set; }
}
