namespace ProductService.Models;

public class Category
{
    public Guid CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    // Quan hệ 1 Category - nhiều Products
    public ICollection<Product> Products { get; set; } = new List<Product>();
}