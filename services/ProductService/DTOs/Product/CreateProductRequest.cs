namespace ProductService.DTOs.Product;

public class CreateProductRequest
{
    public Guid CategoryId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public decimal Price { get; set; }

    public int Stock { get; set; }
}