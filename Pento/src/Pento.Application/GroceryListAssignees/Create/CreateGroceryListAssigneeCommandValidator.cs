using FluentValidation;

namespace Pento.Application.GroceryListAssignees.Create;

public sealed class CreateGroceryListAssigneeCommandValidator : AbstractValidator<CreateGroceryListAssigneeCommand>
{
    public CreateGroceryListAssigneeCommandValidator()
    {
        RuleFor(x => x.GroceryListId)
            .NotEmpty()
            .WithMessage("GroceryListId is required.");

        RuleFor(x => x.HouseholdMemberId)
            .NotEmpty()
            .WithMessage("HouseholdMemberId is required.");
    }
}
