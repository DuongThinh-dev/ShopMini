using System.ComponentModel.DataAnnotations;

namespace UserService.DTOs.Requests;

public class UpdateUserRequestDto
{
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
}