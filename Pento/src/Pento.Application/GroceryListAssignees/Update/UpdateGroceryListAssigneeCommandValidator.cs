using FluentValidation;

namespace Pento.Application.GroceryListAssignees.Update;

internal sealed class UpdateGroceryListAssigneeCommandValidator : AbstractValidator<UpdateGroceryListAssigneeCommand>
{
    public UpdateGroceryListAssigneeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Assignee ID is required.");
    }
}
