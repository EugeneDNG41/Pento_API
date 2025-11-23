using FluentValidation;

namespace Pento.Application.FoodItems.Create;

internal sealed class CreateFoodItemBulkCommandValidator
    : AbstractValidator<CreateFoodItemBulkCommand>
{
    public CreateFoodItemBulkCommandValidator()
    {
        RuleForEach(x => x.Commands).SetValidator(new CreateFoodItemCommandValidator());
    }
}
