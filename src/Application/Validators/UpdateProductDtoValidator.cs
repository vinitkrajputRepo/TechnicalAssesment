using Application.DTOs;
using FluentValidation;

namespace Application.Validators;

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator()
    {
        RuleFor(x => x.ProductName)
            .MaximumLength(255).When(x => !string.IsNullOrEmpty(x.ProductName))
            .WithMessage("Product name cannot exceed 255 characters.");

        RuleFor(x => x.ModifiedBy)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.ModifiedBy))
            .WithMessage("Modified by cannot exceed 100 characters.");
    }
}
