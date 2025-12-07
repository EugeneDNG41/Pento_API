using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Recipes;

namespace Pento.Application.Recipes.Get;

internal sealed class GetRecipeQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory
) : IQueryHandler<GetRecipeQuery, RecipeDetailResponse>
{
    public async Task<Result<RecipeDetailResponse>> Handle(GetRecipeQuery query, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);

        const string recipeSql = """
            SELECT id AS RecipeId, 
                title AS RecipeTitle, 
                description AS Description,
                prep_time_minutes AS PrepTimeMinutes,
                cook_time_minutes AS CookTimeMinutes,
                (prep_time_minutes + cook_time_minutes) AS TotalTimes,
                notes AS Notes,
                servings AS Servings,
                difficulty_level AS DifficultyLevel,
                image_url AS ImageUrl,
                created_by AS CreatedBy,
                is_public AS IsPublic,
                created_on_utc AS CreatedOnUtc,
                updated_on_utc AS UpdatedOnUtc
            FROM recipes
            WHERE id = @RecipeId
            """;

        RecipeDetailResponse? recipe = await connection.QuerySingleOrDefaultAsync<RecipeDetailResponse>(
            recipeSql, new { query.RecipeId });

        if (recipe is null)
        {
            return Result.Failure<RecipeDetailResponse>(RecipeErrors.NotFound);
        }

        string include = query.Include?.ToLowerInvariant() ?? string.Empty;

        if (include is "ingredients" or "all")
        {
            const string ingredientSql = """
                SELECT
                    ri.id AS IngredientId,
                    ri.food_ref_id AS FoodRefId,
                    fr.name AS FoodRefName,
                    fr.image_url AS ImageUrl,
                    ri.quantity AS Quantity,
                    ri.unit_id AS UnitId,
                    u.name AS UnitName,
                    ri.notes AS Notes
                FROM recipe_ingredients ri
                LEFT JOIN food_references fr ON ri.food_ref_id = fr.id
                LEFT JOIN units u ON ri.unit_id = u.id
                WHERE ri.recipe_id = @RecipeId
                ORDER BY fr.name;
                """;

            IEnumerable<RecipeIngredientItem> ingredients = await connection.QueryAsync<RecipeIngredientItem>(
                ingredientSql, new { query.RecipeId });

            recipe.Ingredients.AddRange(ingredients);
        }

        if (include is "directions" or "all")
        {
            const string directionSql = """
                SELECT
                    rd.id AS DirectionId,
                    rd.step_number AS StepNumber,
                    rd.description AS Description,
                    rd.image_url AS ImageUrl
                FROM recipe_directions rd
                WHERE rd.recipe_id = @RecipeId
                ORDER BY rd.step_number;
                """;

            IEnumerable<RecipeDirectionItem> directions = await connection.QueryAsync<RecipeDirectionItem>(
                directionSql, new { query.RecipeId });

            recipe.Directions.AddRange(directions);
        }

        return recipe;
    }
}
