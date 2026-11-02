using ProductService.DTOs.Category;
using ProductService.Models;
using ProductService.Repositories.Interfaces;
using ProductService.Services.Interfaces;

namespace ProductService.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    // Lấy tất cả danh mục
    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        return await _categoryRepository.GetAllAsync();
    }

    // Tìm danh mục theo ID
    public async Task<Category?> GetCategoryByIdAsync(Guid id)
    {
        return await _categoryRepository.GetByIdAsync(id);
    }

    // Tạo danh mục
    public async Task<Category> CreateCategoryAsync(
        CreateCategoryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CategoryName))
            throw new ArgumentException("Tên danh mục không được để trống!");

        string categoryName = request.CategoryName.Trim();

        if (await _categoryRepository.ExistsByNameAsync(categoryName))
            throw new InvalidOperationException("Tên danh mục đã tồn tại!");

        var category = new Category
        {
            CategoryName = categoryName,
            Description = request.Description?.Trim(),
            IsActive = true
        };

        await _categoryRepository.AddAsync(category);

        await _categoryRepository.SaveChangesAsync();

        return category;
    }

    // Cập nhật danh mục
    public async Task<Category?> UpdateCategoryAsync(
        Guid id,
        UpdateCategoryRequest request)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category == null)
            return null;

        if (string.IsNullOrWhiteSpace(request.CategoryName))
            throw new ArgumentException("Tên danh mục không được để trống!");

        string categoryName = request.CategoryName.Trim();

        if (await _categoryRepository.ExistsByNameAsync(categoryName, id))
            throw new InvalidOperationException("Tên danh mục đã tồn tại!");

        category.CategoryName = categoryName;
        category.Description = request.Description?.Trim();
        category.IsActive = request.IsActive;

        await _categoryRepository.UpdateAsync(category);

        await _categoryRepository.SaveChangesAsync();

        return category;
    }

    // Xóa danh mục
    public async Task<bool> DeleteCategoryAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category == null)
            return false;

        if (category.Products.Any())
            throw new InvalidOperationException(
                "Không thể xóa danh mục đang có sản phẩm!");

        await _categoryRepository.DeleteAsync(category);

        await _categoryRepository.SaveChangesAsync();

        return true;
    }

    
}