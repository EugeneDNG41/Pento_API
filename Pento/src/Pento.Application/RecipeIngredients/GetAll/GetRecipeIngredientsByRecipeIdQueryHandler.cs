using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.RecipeIngredients;
using Pento.Domain.Recipes;

namespace Pento.Application.RecipeIngredients.GetAll;

internal sealed class GetRecipeIngredientsByRecipeIdQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    IGenericRepository<Recipe> recipeRepository
    )
    : IQueryHandler<GetRecipeIngredientsByRecipeIdQuery, RecipeWithIngredientsResponse>
{
    public async Task<Result<RecipeWithIngredientsResponse>> Handle(
        GetRecipeIngredientsByRecipeIdQuery query,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync();

        Recipe? recipe = await recipeRepository.GetByIdAsync(query.RecipeId, cancellationToken);
        if (recipe is null)
        {
            return Result.Failure<RecipeWithIngredientsResponse>(RecipeErrors.NotFound);
        }

        const string sql =
            $"""
            SELECT
                r.id AS {nameof(RecipeWithIngredientsResponse.RecipeId)},
                r.title AS {nameof(RecipeWithIngredientsResponse.RecipeTitle)},
                r.description AS {nameof(RecipeWithIngredientsResponse.Description)},
                ri.id AS {nameof(RecipeIngredientItemResponse.IngredientId)},
                ri.food_ref_id AS {nameof(RecipeIngredientItemResponse.FoodRefId)},
                fr.name AS {nameof(RecipeIngredientItemResponse.FoodRefName)},
                ri.quantity AS {nameof(RecipeIngredientItemResponse.Quantity)},
                ri.unit_id AS {nameof(RecipeIngredientItemResponse.UnitId)},
                u.name AS {nameof(RecipeIngredientItemResponse.UnitName)},
                ri.notes AS {nameof(RecipeIngredientItemResponse.Notes)}
            FROM recipes r
            LEFT JOIN recipe_ingredients ri ON ri.recipe_id = r.id
            LEFT JOIN food_references fr ON fr.id = ri.food_ref_id
            LEFT JOIN units u ON u.id = ri.unit_id
            WHERE r.id = @RecipeId;
            """;

        var recipeDict = new Dictionary<Guid, RecipeWithIngredientsResponse>();
        CommandDefinition command = new(sql, new { query.RecipeId }, cancellationToken: cancellationToken);

        await connection.QueryAsync<RecipeWithIngredientsResponse, RecipeIngredientItemResponse, RecipeWithIngredientsResponse>(
            command,
            (recipe, ingredient) =>
            {
                if (!recipeDict.TryGetValue(recipe.RecipeId, out RecipeWithIngredientsResponse? existingRecipe))
                {
                    existingRecipe = recipe;
                    recipeDict.Add(recipe.RecipeId, existingRecipe);
                }

                if (ingredient is not null)
                {
                    existingRecipe.Ingredients.Add(ingredient);
                }

                return existingRecipe;
            },
            splitOn: nameof(RecipeIngredientItemResponse.IngredientId));

        RecipeWithIngredientsResponse? response = recipeDict.Values.FirstOrDefault();

        if (response is null)
        {
            return Result.Failure<RecipeWithIngredientsResponse>(RecipeErrors.NotFound);
        }

        return response;
    }
}
