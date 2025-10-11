using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Recipes;

namespace Pento.Application.Recipes.Get;
internal sealed class GetRecipeQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetRecipeQuery, RecipeResponse>
{
    public async Task<Result<RecipeResponse>> Handle(
        GetRecipeQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync();

        const string sql = """
            SELECT
                id AS Id,
                title AS Title,
                description AS Description,
                prep_time_minutes AS PrepTimeMinutes,
                cook_time_minutes AS CookTimeMinutes,
                notes AS Notes,
                servings AS Servings,
                difficulty_level AS DifficultyLevel,
                calories_per_serving AS CaloriesPerServing,
                image_url AS ImageUrl,
                created_by AS CreatedBy,
                is_public AS IsPublic,
                created_on_utc AS CreatedOnUtc,
                updated_on_utc AS UpdatedOnUtc
            FROM recipes
            WHERE id = @RecipeId
            """;

        RecipeResponse? recipe = await connection.QuerySingleOrDefaultAsync<RecipeResponse>(
            sql,
            new { recipeid = request.RecipeId }
        );

        if (recipe is null)
        {
            return Result.Failure<RecipeResponse>(RecipeErrors.NotFound);
        }

        return recipe;
    }
}
