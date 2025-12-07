using FluentValidation;

namespace Pento.Application.FoodItems.Create;

internal sealed class CreateFoodItemCommandValidator : AbstractValidator<CreateFoodItemCommand>
{
    public CreateFoodItemCommandValidator()
    {
        RuleFor(x => x.Name).MaximumLength(200)
            .WithMessage("Name must not exceed 200 characters.");
        RuleFor(x => x.FoodReferenceId).NotEmpty().WithMessage("Food reference Id is required.");
        RuleFor(x => x.CompartmentId).NotEmpty().WithMessage("Compartment Id is required.");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        RuleFor(x => x.ExpirationDate!.Value)
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
                .When(x => x.ExpirationDate.HasValue)
                .WithMessage("Expiration date must not be in the past.");
    }
}
