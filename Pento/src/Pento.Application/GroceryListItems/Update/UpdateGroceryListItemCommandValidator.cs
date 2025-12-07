using FluentValidation;

namespace Pento.Application.GroceryListItems.Update;

public sealed class UpdateGroceryListItemCommandValidator : AbstractValidator<UpdateGroceryListItemCommand>
{
    public UpdateGroceryListItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0.");

        RuleFor(x => x.Priority)
            .NotEmpty()
            .WithMessage("Priority must be one of: Low, Medium, High.");

        RuleFor(x => x.Notes)
            .MaximumLength(500);

        RuleFor(x => x.CustomName)
            .MaximumLength(100);

        RuleFor(x => x.EstimatedPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.EstimatedPrice.HasValue)
            .WithMessage("Estimated price must be non-negative.");
    }
}
