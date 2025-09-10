using Application.DTOs;
using Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace Application.Tests.Validators;

public class CreateProductDtoValidatorTests
{
    private readonly CreateProductDtoValidator _validator;

    public CreateProductDtoValidatorTests()
    {
        _validator = new CreateProductDtoValidator();
    }

    [Fact]
    public void Should_Pass_When_Valid_Product()
    {
        // Arrange
        var product = new CreateProductDto(
            "Valid Product Name",
            "John Doe"
        );

        // Act
        var result = _validator.TestValidate(product);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_ProductName_Is_Empty()
    {
        // Arrange
        var product = new CreateProductDto(
            "",
            "John Doe"
        );

        // Act
        var result = _validator.TestValidate(product);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProductName);
    }

    [Fact]
    public void Should_Fail_When_ProductName_Exceeds_MaxLength()
    {
        // Arrange
        var longName = new string('A', 256); // 256 characters
        var product = new CreateProductDto(
            longName,
            "John Doe"
        );

        // Act
        var result = _validator.TestValidate(product);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProductName);
    }

    [Fact]
    public void Should_Fail_When_CreatedBy_Is_Empty()
    {
        // Arrange
        var product = new CreateProductDto(
            "Valid Product Name",
            ""
        );

        // Act
        var result = _validator.TestValidate(product);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CreatedBy);
    }

    [Fact]
    public void Should_Fail_When_CreatedBy_Exceeds_MaxLength()
    {
        // Arrange
        var longCreatedBy = new string('A', 101); // 101 characters
        var product = new CreateProductDto(
            "Valid Product Name",
            longCreatedBy
        );

        // Act
        var result = _validator.TestValidate(product);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CreatedBy);
    }
}
