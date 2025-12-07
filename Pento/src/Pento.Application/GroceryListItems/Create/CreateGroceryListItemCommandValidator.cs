using FluentValidation;

namespace Pento.Application.GroceryListItems.Create;

public sealed class CreateGroceryListItemCommandValidator
    : AbstractValidator<CreateGroceryListItemCommand>
{
    public CreateGroceryListItemCommandValidator()
    {
        RuleFor(x => x.ListId)
            .NotEmpty()
            .WithMessage("ListId is required.");

        RuleFor(x => x.FoodRefId)
            .NotEmpty()
            .WithMessage("FoodRefId is required.");


        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0.");

        RuleFor(x => x.Priority)
            .NotEmpty()
            .WithMessage("Priority must be one of: Low, Medium, High.");

        RuleFor(x => x.EstimatedPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.EstimatedPrice.HasValue)
            .WithMessage("EstimatedPrice must be non-negative.");

        RuleFor(x => x.CustomName)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.CustomName))
            .WithMessage("CustomName must not exceed 100 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Notes))
            .WithMessage("Notes must not exceed 500 characters.");
    }


}
