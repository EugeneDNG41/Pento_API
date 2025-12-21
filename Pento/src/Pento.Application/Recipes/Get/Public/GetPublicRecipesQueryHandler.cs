using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.Abstractions;

namespace Pento.Application.Recipes.Get.Public;
internal sealed class GetPublicRecipesQueryHandler(
    ISqlConnectionFactory factory
) : IQueryHandler<GetPublicRecipesQuery, PagedList<RecipeResponse>>
{
    public async Task<Result<PagedList<RecipeResponse>>> Handle(
        GetPublicRecipesQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection =
            await factory.OpenConnectionAsync(cancellationToken);

        var filters = new List<string>
        {
            "r.is_deleted = false",
            "r.is_public = true"
        };

        var parameters = new DynamicParameters();

        if (request.DifficultyLevel is not null)
        {
            filters.Add("r.difficulty_level = @DifficultyLevel");
            parameters.Add("DifficultyLevel", request.DifficultyLevel.ToString());
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            filters.Add("(r.title ILIKE @Search OR r.description ILIKE @Search)");
            parameters.Add("Search", $"%{request.Search}%");
        }

        string whereClause = "WHERE " + string.Join(" AND ", filters);

        string orderBy = request.Sort?.ToLower(System.Globalization.CultureInfo.CurrentCulture) switch
        {
            "oldest" => "ORDER BY r.created_on_utc ASC",
            "title" => "ORDER BY r.title ASC",
            "title_desc" => "ORDER BY r.title DESC",
            _ => "ORDER BY r.created_on_utc DESC"
        };

        string sql = $@"
            SELECT COUNT(*)
            FROM recipes r
            {whereClause};

            SELECT
                r.id AS Id,
                r.title AS Title,
                r.description AS Description,
                r.prep_time_minutes AS PrepTimeMinutes,
                r.cook_time_minutes AS CookTimeMinutes,
                (r.prep_time_minutes + r.cook_time_minutes) AS TotalTimes,
                r.notes AS Notes,
                r.servings AS Servings,
                r.difficulty_level AS DifficultyLevel,
                r.image_url AS ImageUrl,
                r.created_by AS CreatedBy,
                r.is_public AS IsPublic,
                r.created_on_utc AS CreatedOnUtc,
                r.updated_on_utc AS UpdatedOnUtc,
                FALSE AS AddedToWishlist
            FROM recipes r
            {whereClause}
            {orderBy}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        ";

        parameters.Add("Offset", (request.PageNumber - 1) * request.PageSize);
        parameters.Add("PageSize", request.PageSize);

        using SqlMapper.GridReader multi = await connection.QueryMultipleAsync(sql, parameters);

        int totalCount = await multi.ReadFirstAsync<int>();
        var items = (await multi.ReadAsync<RecipeResponse>()).ToList();

        return Result.Success(
            PagedList<RecipeResponse>.Create(
                items,
                totalCount,
                request.PageNumber,
                request.PageSize
            )
        );
    }
}
