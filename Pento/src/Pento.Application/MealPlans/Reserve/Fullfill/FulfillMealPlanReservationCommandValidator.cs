using FluentValidation;

namespace Pento.Application.MealPlans.Reserve.Fullfill;

internal sealed class FulfillMealPlanReservationCommandValidator
    : AbstractValidator<FulfillMealPlanReservationCommand>
{
    public FulfillMealPlanReservationCommandValidator()
    {
        RuleFor(x => x.ReservationId).NotEmpty();
        RuleFor(x => x.NewQuantity).GreaterThan(0);
        RuleFor(x => x.UnitId).NotEmpty();
    }
}
