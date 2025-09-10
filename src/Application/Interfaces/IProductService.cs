using Application.DTOs;

namespace Application.Interfaces;

public interface IProductService
{
    Task<ProductDto> GetByIdAsync(int id);
    Task<ProductListDto> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, string? category = null);
    Task<ProductDto> CreateAsync(CreateProductDto createProductDto);
    Task<ProductDto> UpdateAsync(int id, UpdateProductDto updateProductDto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
