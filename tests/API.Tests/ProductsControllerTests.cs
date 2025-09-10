using System.Net;
using System.Text;
using System.Text.Json;
using API.Controllers;
using Application.DTOs;
using Application.Interfaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Data;
using Moq;
using Xunit;

namespace API.Tests;

public class ProductsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IProductService> _mockProductService;

    public ProductsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _mockProductService = new Mock<IProductService>();
    }

    [Fact]
    public async Task GetProducts_ReturnsOkResult_WithValidParameters()
    {
        // Arrange
        var expectedProducts = new ProductListDto(
            new List<ProductDto>
            {
                new(1, "Test Product", "John Doe", DateTime.UtcNow, null, null)
            },
            1, 1, 10
        );

        _mockProductService.Setup(x => x.GetAllAsync(1, 10, null, null))
            .ReturnsAsync(expectedProducts);

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped<IProductService>(_ => _mockProductService.Object);
            });
        }).CreateClient();

        // Act
        var response = await client.GetAsync("/api/products");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ProductListDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Products);
    }

    [Fact]
    public async Task GetProduct_ReturnsOkResult_WithValidId()
    {
        // Arrange
        var expectedProduct = new ProductDto(1, "Test Product", "John Doe", DateTime.UtcNow, null, null);

        _mockProductService.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(expectedProduct);

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped<IProductService>(_ => _mockProductService.Object);
            });
        }).CreateClient();

        // Act
        var response = await client.GetAsync("/api/products/1");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ProductDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        Assert.NotNull(result);
        Assert.Equal(expectedProduct.ProductName, result.ProductName);
    }

    [Fact]
    public async Task GetProduct_ReturnsNotFound_WithInvalidId()
    {
        // Arrange
        _mockProductService.Setup(x => x.GetByIdAsync(999))
            .ThrowsAsync(new DomainException("Product with ID 999 not found."));

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped<IProductService>(_ => _mockProductService.Object);
            });
        }).CreateClient();

        // Act
        var response = await client.GetAsync("/api/products/999");

        // Assert
        // The controller now properly catches DomainException and returns NotFound
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_ReturnsCreatedResult_WithValidData()
    {
        // Arrange
        var createProductDto = new CreateProductDto("New Product", "Jane Doe");
        var createdProduct = new ProductDto(2, "New Product", "Jane Doe", DateTime.UtcNow, null, null);

        _mockProductService.Setup(x => x.CreateAsync(It.IsAny<CreateProductDto>()))
            .ReturnsAsync(createdProduct);

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped<IProductService>(_ => _mockProductService.Object);
            });
        }).CreateClient();

        var json = JsonSerializer.Serialize(createProductDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/products", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ProductDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        Assert.NotNull(result);
        Assert.Equal(createdProduct.ProductName, result.ProductName);
    }

    [Fact]
    public async Task UpdateProduct_ReturnsOkResult_WithValidData()
    {
        // Arrange
        var updateProductDto = new UpdateProductDto("Updated Product", "Jane Doe");
        var updatedProduct = new ProductDto(1, "Updated Product", "John Doe", DateTime.UtcNow, "Jane Doe", DateTime.UtcNow);

        _mockProductService.Setup(x => x.UpdateAsync(1, It.IsAny<UpdateProductDto>()))
            .ReturnsAsync(updatedProduct);

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped<IProductService>(_ => _mockProductService.Object);
            });
        }).CreateClient();

        var json = JsonSerializer.Serialize(updateProductDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await client.PutAsync("/api/products/1", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ProductDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        Assert.NotNull(result);
        Assert.Equal(updatedProduct.ProductName, result.ProductName);
    }

    [Fact]
    public async Task DeleteProduct_ReturnsNoContent_WithValidId()
    {
        // Arrange
        _mockProductService.Setup(x => x.DeleteAsync(1))
            .ReturnsAsync(true);

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped<IProductService>(_ => _mockProductService.Object);
            });
        }).CreateClient();

        // Act
        var response = await client.DeleteAsync("/api/products/1");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_ReturnsNotFound_WithInvalidId()
    {
        // Arrange
        _mockProductService.Setup(x => x.DeleteAsync(999))
            .ReturnsAsync(false);

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped<IProductService>(_ => _mockProductService.Object);
            });
        }).CreateClient();

        // Act
        var response = await client.DeleteAsync("/api/products/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
