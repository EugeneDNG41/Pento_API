using FluentValidation;

namespace Pento.Application.Recipes.Create;

public sealed class CreateRecipeCommandValidator : AbstractValidator<CreateRecipeCommand>
{
    public CreateRecipeCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Recipe title is required.")
            .MaximumLength(200)
            .WithMessage("Recipe title must be 200 characters or fewer.");

        RuleFor(x => x.PrepTimeMinutes)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Preparation time must be non-negative.");

        RuleFor(x => x.CookTimeMinutes)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Cooking time must be non-negative.");

        RuleFor(x => x.Servings)
            .GreaterThan(0)
            .WithMessage("Servings must be greater than zero.");


        RuleFor(x => x.ImageUrl)
            .Must(uri => uri == null || uri.IsAbsoluteUri)
            .WithMessage("Image url must be a valid absolute URL if provided.");

    }
}
