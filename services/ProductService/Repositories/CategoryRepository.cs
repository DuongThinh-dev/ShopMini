using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Models;
using ProductService.Repositories.Interfaces;

namespace ProductService.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ProductDbContext _context;

    public CategoryRepository(ProductDbContext context)
    {
        _context = context;
    }

    // Lấy tất cả danh mục
    public async Task<List<Category>> GetAllAsync()
    {
        return await _context.Categories.ToListAsync();
    }

    // Tìm danh mục theo ID
    public async Task<Category?> GetByIdAsync(Guid id)
    {
        return await _context.Categories.FindAsync(id);
    }

    // Thêm danh mục
    public async Task AddAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
    }

    // Cập nhật danh mục
    public Task UpdateAsync(Category category)
    {
        _context.Categories.Update(category);

        return Task.CompletedTask;
    }

    // Xóa danh mục
    public Task DeleteAsync(Category category)
    {
        _context.Categories.Remove(category);

        return Task.CompletedTask;
    }

    // Lưu thay đổi
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}