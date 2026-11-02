namespace ProductService.DTOs.Category;

public class UpdateCategoryRequest
{
    public string CategoryName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; }
}