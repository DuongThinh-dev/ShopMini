using ProductService.DTOs.Product;
using ProductService.Models;

namespace ProductService.Services.Interfaces;

public interface IProductService
{
    Task<List<Product>> GetAllProductsAsync();

    Task<Product?> GetProductByIdAsync(Guid id);

    Task<Product> CreateProductAsync(CreateProductRequest request);

    Task<Product?> UpdateProductAsync(
        Guid id,
        UpdateProductRequest request);

    Task<bool> DeleteProductAsync(Guid id);
}