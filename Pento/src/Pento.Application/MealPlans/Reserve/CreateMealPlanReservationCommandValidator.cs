using FluentValidation;

namespace Pento.Application.MealPlans.Reserve;

internal sealed class CreateMealPlanReservationCommandValidator
    : AbstractValidator<CreateMealPlanReservationCommand>
{
    public CreateMealPlanReservationCommandValidator()
    {
        RuleFor(x => x.FoodItemId)
            .NotEmpty();

        RuleFor(x => x.MealPlanId)
            .NotEmpty();

        RuleFor(x => x.UnitId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}
