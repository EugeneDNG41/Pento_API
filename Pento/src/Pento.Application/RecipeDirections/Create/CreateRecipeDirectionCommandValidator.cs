using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Pento.Application.RecipeDirections.Create;

internal sealed class CreateRecipeDirectionCommandValidator : AbstractValidator<CreateRecipeDirectionCommand>
{
    public CreateRecipeDirectionCommandValidator()
    {
        RuleFor(c => c.RecipeId).NotEmpty();
        RuleFor(c => c.StepNumber).GreaterThan(0);
        RuleFor(c => c.Description).NotEmpty().MaximumLength(1000);
        RuleFor(c => c.ImageUrl)
        .Must(url => url == null || url.IsAbsoluteUri)
        .WithMessage("ImageUrl must be a valid absolute URL if provided.");
    }


}
