using FluentValidation;

namespace Pento.Application.Recipes.Delete;

internal sealed class DeleteRecipeCommandValidator : AbstractValidator<DeleteRecipeCommand>
{
    public DeleteRecipeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Recipe Id is required.");
    }
}
