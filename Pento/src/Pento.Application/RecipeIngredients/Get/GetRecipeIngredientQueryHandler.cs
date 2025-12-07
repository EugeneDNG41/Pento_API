using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.RecipeIngredients;

namespace Pento.Application.RecipeIngredients.Get;

internal sealed class GetRecipeIngredientQueryHandler(ISqlConnectionFactory sqlConnectionFactory, IGenericRepository<RecipeIngredient> RecipeIngredientRepository)
    : IQueryHandler<GetRecipeIngredientQuery, RecipeIngredientResponse>
{
    public async Task<Result<RecipeIngredientResponse>> Handle(
        GetRecipeIngredientQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);

        RecipeIngredient? recipeIngredient = await RecipeIngredientRepository.GetByIdAsync(request.RecipeIngredientId, cancellationToken);
        if (recipeIngredient == null)
        {
            return Result.Failure<RecipeIngredientResponse>(RecipeIngredientErrors.NotFound);
        }
        const string sql =
            $"""
            SELECT
                id AS {nameof(RecipeIngredientResponse.Id)},
                recipe_id AS {nameof(RecipeIngredientResponse.RecipeId)},
                foodref_id AS {nameof(RecipeIngredientResponse.FoodRefId)},
                quantity AS {nameof(RecipeIngredientResponse.Quantity)},
                unit_id AS {nameof(RecipeIngredientResponse.UnitId)},
                notes AS {nameof(RecipeIngredientResponse.Notes)},
                created_on_utc AS {nameof(RecipeIngredientResponse.CreatedOnUtc)},
                updated_on_utc AS {nameof(RecipeIngredientResponse.UpdatedOnUtc)}
            FROM recipe_ingredients
            WHERE id = @RecipeIngredientId
            """;
        CommandDefinition command = new(
            sql,
            new { recipeIngredientId = request.RecipeIngredientId },
            cancellationToken: cancellationToken
        );
        RecipeIngredientResponse? ingredient = await connection.QuerySingleOrDefaultAsync<RecipeIngredientResponse>(command);


        if (ingredient is null)
        {
            return Result.Failure<RecipeIngredientResponse>(RecipeIngredientErrors.NotFound);
        }

        return ingredient;
    }
}
