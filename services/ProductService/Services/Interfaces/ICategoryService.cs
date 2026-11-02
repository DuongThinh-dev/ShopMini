using ProductService.DTOs.Category;
using ProductService.Models;

namespace ProductService.Services.Interfaces;

public interface ICategoryService
{
    Task<List<Category>> GetAllCategoriesAsync();

    Task<Category?> GetCategoryByIdAsync(Guid id);

    Task<Category> CreateCategoryAsync(CreateCategoryRequest request);

    Task<Category?> UpdateCategoryAsync(
        Guid id,
        UpdateCategoryRequest request);

    Task<bool> DeleteCategoryAsync(Guid id);
}