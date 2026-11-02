using ProductService.Models;

namespace ProductService.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();

    Task<Category?> GetByIdAsync(Guid id);

    Task AddAsync(Category category);

    Task UpdateAsync(Category category);

    Task DeleteAsync(Category category);

    Task SaveChangesAsync();

    // Kiểm tra tên danh mục đã tồn tại
    Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null);
}