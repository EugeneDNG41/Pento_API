using FluentValidation;

namespace Pento.Application.FoodItems.UpdateExpirationDate;

internal sealed class UpdateFoodItemExpirationDateCommandValidator : AbstractValidator<UpdateFoodItemExpirationDateCommand>
{
    public UpdateFoodItemExpirationDateCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Food item Id must not be empty.");
        RuleFor(x => x.NewExpirationDate.ToUniversalTime())
            .GreaterThan(DateTime.UtcNow).WithMessage("Expiration date must be in the future.");
    }
}
