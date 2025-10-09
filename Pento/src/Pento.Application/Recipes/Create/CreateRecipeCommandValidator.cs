using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Pento.Application.Recipes.Create;
public sealed class CreateRecipeCommandValidator : AbstractValidator<CreateRecipeCommand>
{
    public CreateRecipeCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("The recipe title cannot be empty.")
            .MaximumLength(200)
            .WithMessage("The recipe title must be 200 characters or fewer.");

        RuleFor(x => x.PrepTimeMinutes)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Preparation time must be non-negative.");

        RuleFor(x => x.CookTimeMinutes)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Cooking time must be non-negative.");

        RuleFor(x => x.Servings)
            .GreaterThan(0)
            .WithMessage("Servings must be greater than zero.");

        RuleFor(x => x.DifficultyLevel)
            .Must(d => string.IsNullOrEmpty(d))
            .WithMessage("Invalid difficulty level. Allowed values: Easy, Medium, Hard.");

        RuleFor(x => x.CreatedBy)
            .NotEmpty()
            .WithMessage("CreatedBy user ID is required.");

        RuleFor(x => x.ImageUrl)
            .Must(uri => uri == null || uri.IsAbsoluteUri)
            .WithMessage("ImageUrl must be a valid absolute URL if provided.");

    }
}
