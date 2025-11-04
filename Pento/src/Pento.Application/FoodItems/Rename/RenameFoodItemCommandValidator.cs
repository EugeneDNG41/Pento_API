using FluentValidation;

namespace Pento.Application.FoodItems.Rename;

internal sealed class RenameFoodItemCommandValidator : AbstractValidator<RenameFoodItemCommand>
{
    public RenameFoodItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Food item Id must not be empty.");
        RuleFor(x => x.Name)
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");
    }
}
