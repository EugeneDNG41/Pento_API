using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.Abstractions;

namespace Pento.Application.Recipes.Get;

internal sealed class GetAllRecipesQueryHandler(
    IUserContext userContext,
    ISqlConnectionFactory factory)
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

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            filters.Add("(title ILIKE @GetAll OR description ILIKE @GetAll)");
            parameters.Add("GetAll", $"%{request.Search}%");
        }

        filters.Add("is_deleted = false");

        string whereClause = filters.Count > 0
            ? "WHERE " + string.Join(" AND ", filters)
            : string.Empty;

        string orderBy = request.Sort?.ToLower(System.Globalization.CultureInfo.CurrentCulture) switch
        {
            "oldest" => "ORDER BY created_on_utc ASC",
            "title" => "ORDER BY title ASC",
            "title_desc" => "ORDER BY title DESC",
            _ => "ORDER BY created_on_utc DESC"
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
                CASE
                    WHEN @IsAuthenticated = FALSE THEN FALSE
                    ELSE EXISTS (
                        SELECT 1
                        FROM recipe_wishlists w
                        WHERE w.recipe_id = r.id
                          AND w.user_id = @UserId
                    )
                END AS AddedToWishlist
            FROM recipes r
            {whereClause}
            {orderBy}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;


        ";
        bool isAuthenticated = false;
        if (userContext.UserId != Guid.Empty)
        {
             isAuthenticated = true;
        }
        parameters.Add("IsAuthenticated", isAuthenticated);
        parameters.Add("UserId", userContext.UserId);
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

