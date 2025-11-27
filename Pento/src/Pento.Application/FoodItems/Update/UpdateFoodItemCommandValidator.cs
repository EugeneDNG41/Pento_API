using FluentValidation;

namespace Pento.Application.FoodItems.Update;

internal sealed class UpdateFoodItemCommandValidator : AbstractValidator<UpdateFoodItemCommand>
{
    public UpdateFoodItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Food item Code is required.")
            .When(x => x != null);
        RuleFor(x => x.Name)
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.")
            .When(x => !string.IsNullOrEmpty(x.Name));
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.")
            .When(x => x != null);
        RuleFor(x => x.ExpirationDate)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Expiration date must not be in the past.")
            .When(x => x != null);
        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes must not exceed 500 characters.")
            .When(x => x != null);
    }
}

