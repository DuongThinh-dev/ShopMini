using ProductService.DTOs.Product;
using ProductService.Models;
using ProductService.Repositories.Interfaces;
using ProductService.Services.Interfaces;

namespace ProductService.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    // Lấy tất cả sản phẩm
    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _productRepository.GetAllAsync();
    }

    // Tìm sản phẩm theo ID
    public async Task<Product?> GetProductByIdAsync(Guid id)
    {
        return await _productRepository.GetByIdAsync(id);
    }

    // Tạo sản phẩm
    public async Task<Product> CreateProductAsync(
        CreateProductRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ProductName))
            throw new ArgumentException("Tên sản phẩm không được để trống!");

        if (request.Price <= 0)
            throw new ArgumentException("Giá sản phẩm phải lớn hơn 0!");

        if (request.Stock < 0)
            throw new ArgumentException("Số lượng tồn kho không được âm!");

        // Kiểm tra danh mục tồn tại
        var category = await _categoryRepository.GetByIdAsync(
            request.CategoryId);

        if (category == null)
            throw new InvalidOperationException("Danh mục không tồn tại!");

        if (!category.IsActive)
            throw new InvalidOperationException("Danh mục đã bị vô hiệu hóa!");

        var product = new Product
        {
            CategoryId = request.CategoryId,
            ProductName = request.ProductName.Trim(),
            Description = request.Description?.Trim(),
            ImageUrl = request.ImageUrl?.Trim(),
            Price = request.Price,
            Stock = request.Stock,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _productRepository.AddAsync(product);

        await _productRepository.SaveChangesAsync();

        return product;
    }

    // Cập nhật sản phẩm
    public async Task<Product?> UpdateProductAsync(
        Guid id,
        UpdateProductRequest request)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
            return null;

        if (string.IsNullOrWhiteSpace(request.ProductName))
            throw new ArgumentException("Tên sản phẩm không được để trống!");

        if (request.Price <= 0)
            throw new ArgumentException("Giá sản phẩm phải lớn hơn 0!");

        if (request.Stock < 0)
            throw new ArgumentException("Số lượng tồn kho không được âm!");

        // Kiểm tra danh mục tồn tại
        var category = await _categoryRepository.GetByIdAsync(
            request.CategoryId);

        if (category == null)
            throw new InvalidOperationException("Danh mục không tồn tại!");

        if (!category.IsActive)
            throw new InvalidOperationException("Danh mục đã bị vô hiệu hóa!");

        product.CategoryId = request.CategoryId;
        product.ProductName = request.ProductName.Trim();
        product.Description = request.Description?.Trim();
        product.ImageUrl = request.ImageUrl?.Trim();
        product.Price = request.Price;
        product.Stock = request.Stock;
        product.IsActive = request.IsActive;
        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product);

        await _productRepository.SaveChangesAsync();

        return product;
    }

    // Xóa sản phẩm
    public async Task<bool> DeleteProductAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
            return false;

        await _productRepository.DeleteAsync(product);

        await _productRepository.SaveChangesAsync();

        return true;
    }
}