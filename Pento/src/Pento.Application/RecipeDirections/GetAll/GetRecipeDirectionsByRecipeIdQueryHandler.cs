using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.RecipeDirections;
using Pento.Domain.Recipes;

namespace Pento.Application.RecipeDirections.GetAll;

internal sealed class GetRecipeDirectionsByRecipeIdQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    IGenericRepository<Recipe> recipeRepository
    )
    : IQueryHandler<GetRecipeDirectionsByRecipeIdQuery, RecipeWithDirectionsResponse>
{
    public async Task<Result<RecipeWithDirectionsResponse>> Handle(
        GetRecipeDirectionsByRecipeIdQuery query,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);

        Recipe? recipe = await recipeRepository.GetByIdAsync(query.RecipeId, cancellationToken);
        if (recipe is null)
        {
            return Result.Failure<RecipeWithDirectionsResponse>(RecipeErrors.NotFound);
        }

        const string sql =
            $"""
            SELECT
                r.id AS {nameof(RecipeWithDirectionsResponse.RecipeId)},
                r.title AS {nameof(RecipeWithDirectionsResponse.RecipeTitle)},
                r.description AS {nameof(RecipeWithDirectionsResponse.Description)},
                rd.id AS {nameof(RecipeDirectionItemResponse.DirectionId)},
                rd.step_number AS {nameof(RecipeDirectionItemResponse.StepNumber)},
                rd.description AS {nameof(RecipeDirectionItemResponse.Description)},
                rd.image_url AS {nameof(RecipeDirectionItemResponse.ImageUrl)}
            FROM recipes r
            LEFT JOIN recipe_directions rd ON rd.recipe_id = r.id
            WHERE r.id = @RecipeId
            ORDER BY rd.step_number ASC;
            """;

        var recipeDict = new Dictionary<Guid, RecipeWithDirectionsResponse>();
        CommandDefinition command = new(sql, new { query.RecipeId }, cancellationToken: cancellationToken);

        await connection.QueryAsync<RecipeWithDirectionsResponse, RecipeDirectionItemResponse, RecipeWithDirectionsResponse>(
            command,
            (recipe, direction) =>
            {
                if (!recipeDict.TryGetValue(recipe.RecipeId, out RecipeWithDirectionsResponse? existingRecipe))
                {
                    existingRecipe = recipe;
                    recipeDict.Add(recipe.RecipeId, existingRecipe);
                }

                if (direction is not null)
                {
                    existingRecipe.Directions.Add(direction);
                }

                return existingRecipe;
            },
            splitOn: nameof(RecipeDirectionItemResponse.DirectionId));

        RecipeWithDirectionsResponse? response = recipeDict.Values.FirstOrDefault();

        if (response is null)
        {
            return Result.Failure<RecipeWithDirectionsResponse>(RecipeErrors.NotFound);
        }

        return response;
    }
}
