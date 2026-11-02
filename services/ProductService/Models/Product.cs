namespace ProductService.Models;

public class Product
{
    public Guid ProductId { get; set; }

    public Guid CategoryId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    // Quan hệ nhiều Product - một Category
    public Category Category { get; set; } = null!;
}