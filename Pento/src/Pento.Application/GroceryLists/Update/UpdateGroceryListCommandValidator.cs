using FluentValidation;

namespace Pento.Application.GroceryLists.Update;

internal sealed class UpdateGroceryListCommandValidator : AbstractValidator<UpdateGroceryListCommand>
{
    public UpdateGroceryListCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Grocery list ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Grocery list name is required.")
            .MaximumLength(100).WithMessage("Grocery list name cannot exceed 100 characters.")
            .Matches("^[a-zA-Z0-9\\s\\-_,.]+$")
            .WithMessage("Grocery list name contains invalid characters.");
    }
}
