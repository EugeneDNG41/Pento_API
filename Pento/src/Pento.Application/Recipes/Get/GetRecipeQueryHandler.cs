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

        const string sql =
           $"""
            SELECT
                id AS {nameof(RecipeResponse.Id)},
                title AS {nameof(RecipeResponse.Title)},
                description AS {nameof(RecipeResponse.Description)},
                prep_time_minutes AS {nameof(RecipeResponse.PrepTimeMinutes)},
                cook_time_minutes AS {nameof(RecipeResponse.CookTimeMinutes)},
                notes AS {nameof(RecipeResponse.Notes)},
                servings AS {nameof(RecipeResponse.Servings)},
                difficulty_level AS {nameof(RecipeResponse.DifficultyLevel)},
                image_url AS {nameof(RecipeResponse.ImageUrl)},
                created_by AS {nameof(RecipeResponse.CreatedBy)},
                is_public AS {nameof(RecipeResponse.IsPublic)},
                created_on_utc AS {nameof(RecipeResponse.CreatedOnUtc)},
                updated_on_utc AS {nameof(RecipeResponse.UpdatedOnUtc)}
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
