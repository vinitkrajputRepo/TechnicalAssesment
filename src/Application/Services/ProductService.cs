using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Persistence;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ProductDto> GetByIdAsync(int id)
    {
        _logger.LogInformation("Retrieving product with ID {ProductId}", id);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new DomainException($"Product with ID {id} not found.");

            stopwatch.Stop();
            _logger.LogInformation("Product retrieved successfully in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
            
            return _mapper.Map<ProductDto>(product);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error retrieving product with ID {ProductId} after {ElapsedMs}ms", id, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    public async Task<ProductListDto> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null, string? category = null)
    {
        _logger.LogInformation("Retrieving products - Page: {PageNumber}, Size: {PageSize}, Search: {SearchTerm}", 
            pageNumber, pageSize, searchTerm ?? "None");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {
            var products = await _productRepository.GetAllAsync(pageNumber, pageSize, searchTerm, category);
            var totalCount = await _productRepository.GetTotalCountAsync(searchTerm, category);

            var productDtos = _mapper.Map<List<ProductDto>>(products);

            stopwatch.Stop();
            _logger.LogInformation("Products retrieved successfully - Count: {Count}, Total: {TotalCount}, Time: {ElapsedMs}ms", 
                products.Count(), totalCount, stopwatch.ElapsedMilliseconds);

            return new ProductListDto(productDtos, totalCount, pageNumber, pageSize);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error retrieving products after {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto createProductDto)
    {
        _logger.LogInformation("Creating new product: {ProductName}", createProductDto.ProductName);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {
            var product = _mapper.Map<Product>(createProductDto);
            product.CreatedOn = DateTime.UtcNow;

            await _productRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            stopwatch.Stop();
            _logger.LogInformation("Product created successfully with ID {ProductId} in {ElapsedMs}ms", 
                product.Id, stopwatch.ElapsedMilliseconds);

            return _mapper.Map<ProductDto>(product);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error creating product after {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto updateProductDto)
    {
        _logger.LogInformation("Updating product with ID {ProductId}", id);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new DomainException($"Product with ID {id} not found.");

            if (updateProductDto.ProductName != null)
                product.ProductName = updateProductDto.ProductName;
            if (updateProductDto.ModifiedBy != null)
                product.ModifiedBy = updateProductDto.ModifiedBy;

            product.ModifiedOn = DateTime.UtcNow;

            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            stopwatch.Stop();
            _logger.LogInformation("Product updated successfully in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);

            return _mapper.Map<ProductDto>(product);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error updating product with ID {ProductId} after {ElapsedMs}ms", 
                id, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting product with ID {ProductId}", id);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return false;

            _productRepository.Delete(product);
            await _unitOfWork.SaveChangesAsync();

            stopwatch.Stop();
            _logger.LogInformation("Product deleted successfully in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);

            return true;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error deleting product with ID {ProductId} after {ElapsedMs}ms", 
                id, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {
            var exists = await _productRepository.ExistsAsync(id);
            stopwatch.Stop();
            _logger.LogDebug("Product existence check for ID {ProductId}: {Exists} in {ElapsedMs}ms", 
                id, exists, stopwatch.ElapsedMilliseconds);
            return exists;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error checking product existence for ID {ProductId} after {ElapsedMs}ms", 
                id, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
