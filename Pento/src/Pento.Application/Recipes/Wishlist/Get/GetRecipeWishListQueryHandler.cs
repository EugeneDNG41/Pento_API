using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.RecipeWishLists.GetAll;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;

namespace Pento.Application.Recipes.Wishlist.Get;

internal sealed class GetRecipeWishListQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    IUserContext userContext
) : IQueryHandler<GetRecipeWishListQuery, List<RecipeWishListResponse>>
{
    public async Task<Result<List<RecipeWishListResponse>>> Handle(
        GetRecipeWishListQuery query,
        CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;

        if (householdId is null)
        {
            return Result.Failure<List<RecipeWishListResponse>>(HouseholdErrors.NotInAnyHouseHold);
        }

        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);

        const string sql = @"
            SELECT
                w.id                 AS WishListId,
                w.recipe_id           AS RecipeId,
                r.title              AS Title,
                r.image_url           AS ImageUrl,
                r.difficulty_level    AS DifficultyLevel,
                r.recipe_prep_time_minutes    AS PrepTimeMinutes,
                w.added_on_utc         AS AddedOnUtc
            FROM recipe_wishlists w
            INNER JOIN recipes r ON r.id = w.recipe_id
            WHERE w.household_id = @HouseholdId and w.is_deleted = false
            ORDER BY w.added_on_utc DESC;
        ";

        var items = (await connection.QueryAsync<RecipeWishListResponse>(
            sql,
            new { HouseholdId = householdId.Value }
        )).ToList();

        return items;
    }
}
