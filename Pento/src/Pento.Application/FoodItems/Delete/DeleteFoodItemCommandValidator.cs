using FluentValidation;

namespace Pento.Application.FoodItems.Delete;

internal sealed class DeleteFoodItemCommandValidator : AbstractValidator<DeleteFoodItemCommand>
{
    public DeleteFoodItemCommandValidator()
    {
        RuleFor(x => x.FoodItemId)
            .NotEmpty().WithMessage("Food Item Id is required.");
    }
}
