using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Application.Interfaces.Persistence;

namespace Infrastructure.Data.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Product>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, string? category = null)
    {
        var query = _context.Products
            .Include(p => p.Items)
            .AsNoTracking()
            .AsQueryable();

        // Apply search filter - search in ProductName and CreatedBy
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => 
                p.ProductName.Contains(searchTerm) || 
                p.CreatedBy.Contains(searchTerm));
        }

        // Apply pagination
        var skip = (pageNumber - 1) * pageSize;
        return await query
            .OrderBy(p => p.ProductName)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync(string? searchTerm = null, string? category = null)
    {
        var query = _context.Products.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => 
                p.ProductName.Contains(searchTerm) || 
                p.CreatedBy.Contains(searchTerm));
        }

        return await query.CountAsync();
    }

    public async Task<Product> AddAsync(Product product)
    {
        var result = await _context.Products.AddAsync(product);
        return result.Entity;
    }

    public void Update(Product product)
    {
        _context.Products.Update(product);
    }

    public void Delete(Product product)
    {
        _context.Products.Remove(product);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Products.AsNoTracking().AnyAsync(p => p.Id == id);
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
    {
        var query = _context.Products.AsNoTracking().AsQueryable();
        
        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);
            
        return await query.AnyAsync(p => p.ProductName == name);
    }

    public async Task<IEnumerable<Product>> GetByCreatedByAsync(string createdBy)
    {
        return await _context.Products
            .Include(p => p.Items)
            .AsNoTracking()
            .Where(p => p.CreatedBy == createdBy)
            .OrderBy(p => p.ProductName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetRecentlyModifiedAsync(int days = 7)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);
        return await _context.Products
            .Include(p => p.Items)
            .AsNoTracking()
            .Where(p => p.ModifiedOn.HasValue && p.ModifiedOn >= cutoffDate)
            .OrderByDescending(p => p.ModifiedOn)
            .ToListAsync();
    }
}
