using FluentValidation;

namespace Pento.Application.GroceryLists.Create;

internal sealed class CreateGroceryListCommandValidator : AbstractValidator<CreateGroceryListCommand>
{
    public CreateGroceryListCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Grocery list name is required.")
            .MaximumLength(100).WithMessage("Grocery list name cannot exceed 100 characters.")
            .Matches(@"^[a-zA-Z0-9\s\-_,.()]+$")
            .WithMessage("Grocery list name contains invalid characters.");
    }
}
