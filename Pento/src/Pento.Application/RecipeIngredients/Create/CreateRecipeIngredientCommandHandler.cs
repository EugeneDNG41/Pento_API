using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.RecipeIngredients;

namespace Pento.Application.RecipeIngredients.Create;
internal sealed class CreateRecipeIngredientCommandHandler(
    IRecipeIngredientRepository recipeIngredientRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateRecipeIngredientCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateRecipeIngredientCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0)
        {
            return Result.Failure<Guid>(RecipeIngredientErrors.InvalidQuantity);
        }

        DateTime utcNow = DateTime.UtcNow;

        var ingredient = RecipeIngredient.Create(
            request.RecipeId,
            request.FoodRefId,
            request.Quantity,
            request.UnitId,
            request.Notes,
            utcNow
        );

        await recipeIngredientRepository.AddAsync(ingredient, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(ingredient.Id);
    }
}
