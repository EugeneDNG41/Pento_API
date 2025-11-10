using FluentValidation;

namespace Pento.Application.FoodItems.Update;

internal sealed class UpdateFoodItemCommandValidator : AbstractValidator<UpdateFoodItemCommand>
{
    public UpdateFoodItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Food item Id must not be empty.");
        RuleFor(x => x.CompartmentId)
            .NotEmpty().WithMessage("Compartment Id must not be empty.");
        RuleFor(x => x.UnitId)
            .NotEmpty().WithMessage("Measurement unit Id must not be empty.");
        RuleFor(x => x.Name)
            .NotNull().WithMessage("Name must not be null.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        RuleFor(x => x.ExpirationDateUtc)
            .GreaterThan(DateTime.UtcNow).WithMessage("Expiration date must be in the future.");
        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes must not exceed 500 characters.");
        RuleFor(x => x.Version)
            .GreaterThanOrEqualTo(0).WithMessage("Version must be zero or a positive integer.");
    }
}

