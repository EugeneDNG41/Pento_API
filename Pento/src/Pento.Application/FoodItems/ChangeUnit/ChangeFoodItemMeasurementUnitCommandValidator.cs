using FluentValidation;

namespace Pento.Application.FoodItems.ChangeUnit;

internal sealed class ChangeFoodItemMeasurementUnitCommandValidator : AbstractValidator<ChangeFoodItemMeasurementUnitCommand>
{
    public ChangeFoodItemMeasurementUnitCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Food item Id must not be empty.");
        RuleFor(x => x.MeasurementUnitId)
            .NotEmpty().WithMessage("Measurement unit Id must not be empty.");
    }
}
