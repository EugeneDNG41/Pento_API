using FluentValidation;
using Pento.Domain.GroceryListItems;

namespace Pento.Application.GroceryLists.Create;

public sealed class CreateGroceryListDetailCommandValidator : AbstractValidator<CreateGroceryListDetailCommand>
{
    public CreateGroceryListDetailCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.Quantity).GreaterThan(0);
            item.RuleFor(i => i.Priority)
                .Must(BeValidPriority)
                .WithMessage("Priority must be one of: Low, Medium, High.");
        });

    }

    private static bool BeValidPriority(string priority)
    {
        return Enum.TryParse<GroceryItemPriority>(priority, true, out _);
    }
}
