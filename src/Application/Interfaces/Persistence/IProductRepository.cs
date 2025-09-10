using Domain.Entities;

namespace Application.Interfaces.Persistence;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, string? category = null);
    Task<int> GetTotalCountAsync(string? searchTerm = null, string? category = null);
    Task<Product> AddAsync(Product product);
    void Update(Product product);
    void Delete(Product product);
    Task<bool> ExistsAsync(int id);
}
