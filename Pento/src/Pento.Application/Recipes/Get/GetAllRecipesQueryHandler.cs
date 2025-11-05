using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodReferences.Get;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;

namespace Pento.Application.Recipes.Get;
internal sealed class GetAllRecipesQueryHandler(ISqlConnectionFactory factory)
    : IQueryHandler<GetAllRecipesQuery, IReadOnlyList<RecipeResponse>>
{
    public async Task<Result<IReadOnlyList<RecipeResponse>>> Handle(GetAllRecipesQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await factory.OpenConnectionAsync();

            const string sql = """
            SELECT 
                id AS Id,
                title AS Title,
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
            ORDER BY created_on_utc DESC;
            """;

        IEnumerable<RecipeResponse> recipes = await connection.QueryAsync<RecipeResponse>(sql);

        return Result.Success<IReadOnlyList<RecipeResponse>>(recipes.ToList());
    }
}
