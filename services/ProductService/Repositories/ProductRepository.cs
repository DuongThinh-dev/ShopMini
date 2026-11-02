using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Models;
using ProductService.Repositories.Interfaces;

namespace ProductService.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductDbContext _context;

    public ProductRepository(ProductDbContext context)
    {
        _context = context;
    }

    // Lấy tất cả sản phẩm
    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .ToListAsync();
    }

    // Tìm sản phẩm theo ID
    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ProductId == id);
    }

    // Thêm sản phẩm
    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
    }

    // Cập nhật sản phẩm
    public Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);

        return Task.CompletedTask;
    }

    // Xóa sản phẩm
    public Task DeleteAsync(Product product)
    {
        _context.Products.Remove(product);

        return Task.CompletedTask;
    }

    // Lưu thay đổi
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}