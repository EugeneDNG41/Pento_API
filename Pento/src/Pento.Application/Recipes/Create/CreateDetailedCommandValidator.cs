using FluentValidation;

namespace Pento.Application.Recipes.Create;

public sealed class CreateDetailedCommandValidator : AbstractValidator<CreateDetailedRecipeCommand>
{
    public CreateDetailedCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Recipe title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).When(x => !string.IsNullOrWhiteSpace(x.Description))
            .WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.PrepTimeMinutes)
            .GreaterThanOrEqualTo(0).WithMessage("Prep time must be zero or positive.");

        RuleFor(x => x.CookTimeMinutes)
            .GreaterThanOrEqualTo(0).WithMessage("Cook time must be zero or positive.");

        RuleFor(x => x.Servings)
            .GreaterThan(0).When(x => x.Servings.HasValue)
            .WithMessage("Servings must be greater than zero.");

        RuleFor(x => x.Notes)
            .MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Notes))
            .WithMessage("Notes cannot exceed 500 characters.");

        RuleForEach(x => x.Ingredients).ChildRules(ingredients =>
        {
            ingredients.RuleFor(i => i.FoodRefId)
                .NotEmpty().WithMessage("Food reference ID is required for each ingredient.");

            ingredients.RuleFor(i => i.UnitId)
                .NotEmpty().WithMessage("Unit ID is required for each ingredient.");

            ingredients.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

            ingredients.RuleFor(i => i.Notes)
                .MaximumLength(300)
                .When(i => !string.IsNullOrWhiteSpace(i.Notes))
                .WithMessage("Ingredient notes cannot exceed 300 characters.");
        });

        RuleFor(x => x.Ingredients)
            .NotEmpty().WithMessage("At least one ingredient is required.");

        RuleForEach(x => x.Directions).ChildRules(direction =>
        {
            direction.RuleFor(d => d.StepNumber)
                .GreaterThan(0).WithMessage("Step number must be greater than zero.");

            direction.RuleFor(d => d.Description)
                .NotEmpty().WithMessage("Each step must have a description.")
                .MaximumLength(1000).WithMessage("Step description cannot exceed 1000 characters.");
        });

        RuleFor(x => x.Directions)
            .NotEmpty().WithMessage("At least one direction step is required.")
            .Must(HaveUniqueSteps).WithMessage("Step numbers must be unique.");
    }

    private static bool HaveUniqueSteps(List<RecipeDirectionRequest> directions)
    {
        return directions.Select(d => d.StepNumber).Distinct().Count() == directions.Count;
    }
}

