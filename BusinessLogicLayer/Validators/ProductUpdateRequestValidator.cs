using BusinessLogicLayer.DTO;
using FluentValidation;

namespace BusinessLogicLayer.Validators
{
    public class ProductUpdateRequestValidator : AbstractValidator<ProductUpdateRequest>
    {
        public ProductUpdateRequestValidator()
        {
            RuleFor(x => x.ProductID)
                .NotNull().WithMessage("Product ID must be provided.")
                .NotEmpty().WithMessage("Product ID is required.");
            RuleFor(x => x.ProductName)
                .NotNull().WithMessage("Product name must be provided.")
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.");
            RuleFor(x => x.Category)
                .NotNull().WithMessage("Category must be provided.")
                .IsInEnum().WithMessage("Invalid category.");
            RuleFor(x => x.UnitPrice)
                .NotNull().WithMessage("Unit price must be provided.")
                .InclusiveBetween(0, double.MaxValue).WithMessage($"Unit price must be between 0 and {double.MaxValue}.");
            RuleFor(x => x.QuantityInStock)
                .NotNull().WithMessage("Quantity in stock must be provided.")
                .InclusiveBetween(0, int.MaxValue).WithMessage($"Quantity in stock must be between 0 and {int.MaxValue}.");
        }
    }
}