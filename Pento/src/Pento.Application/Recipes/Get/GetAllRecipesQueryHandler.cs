using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Domain.Abstractions;
using Pento.Domain.Recipes;

namespace Pento.Application.Recipes.Get;

internal sealed class GetAllRecipesQueryHandler(ISqlConnectionFactory factory)
    : IQueryHandler<GetAllRecipesQuery, PagedList<RecipeResponse>>
{
    public async Task<Result<PagedList<RecipeResponse>>> Handle(GetAllRecipesQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await factory.OpenConnectionAsync(cancellationToken);

        var filters = new List<string>();
        var parameters = new DynamicParameters();

        if (request.DifficultyLevel is not null)
        {
            filters.Add("difficulty_level = @DifficultyLevel");
            parameters.Add("DifficultyLevel", request.DifficultyLevel.ToString());
        }

        string whereClause = filters.Count > 0
            ? "WHERE " + string.Join(" AND ", filters)
            : string.Empty;


        string sql = $@"
            SELECT COUNT(*) 
            FROM recipes
            {whereClause};

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
            {whereClause}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        ";

        parameters.Add("Offset", (request.PageNumber - 1) * request.PageSize);
        parameters.Add("PageSize", request.PageSize);

        SqlMapper.GridReader multi = await connection.QueryMultipleAsync(sql, parameters);

        int totalCount = await multi.ReadFirstAsync<int>();
        var items = (await multi.ReadAsync<RecipeResponse>()).ToList();

        var paged = PagedList<RecipeResponse>.Create(
            items,
            totalCount,
            request.PageNumber,
            request.PageSize
        );

        return Result.Success(paged);
    }
}
