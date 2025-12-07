using FluentValidation;

namespace Pento.Application.Recipes.Reserve;

internal sealed class CreateRecipeReservationCommandValidator : AbstractValidator<CreateRecipeReservationCommand>
{
    public CreateRecipeReservationCommandValidator()
    {
        RuleFor(x => x.RecipeId).NotEmpty();
        RuleFor(x => x.Quantity).
            GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        RuleFor(x => x.UnitId).NotEmpty();
        RuleFor(x => x.FoodItemId).NotEmpty();
    }
}
