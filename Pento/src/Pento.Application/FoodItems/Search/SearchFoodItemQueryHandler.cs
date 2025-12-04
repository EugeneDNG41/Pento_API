using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;
using Pento.Domain.Households;

namespace Pento.Application.FoodItems.Search;

internal sealed class SearchFoodItemQueryHandler(
    IUserContext userContext,
    ISqlConnectionFactory sqlConnectionFactory
    ) : IQueryHandler<SearchFoodItemQuery, PagedList<FoodItemPreview>>
{
    public async Task<Result<PagedList<FoodItemPreview>>> Handle(SearchFoodItemQuery query, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<PagedList<FoodItemPreview>>(HouseholdErrors.NotInAnyHouseHold);
        }
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        string orderBy = query.SortBy switch
        {
            FoodItemPreviewSortBy.Name => "2",
            FoodItemPreviewSortBy.FoodGroup => "3",
            FoodItemPreviewSortBy.Quantity => "5",
            FoodItemPreviewSortBy.ExpirationDate => "7",
            FoodItemPreviewSortBy.Default or _ => "1"
        };
        string orderClause = $"ORDER BY {orderBy} {query.SortOrder}";
        var andFilters = new List<string>
        {
            "fi.is_deleted IS FALSE",
            "fi.household_id = @HouseholdId"
        };
        var orFoodGroupFilters = new List<string>();
        var orStatusFilters = new List<string>();
        var parameters = new DynamicParameters();
        parameters.Add("HouseholdId", householdId);
        if (query.FoodReferenceId  != null)
        {
            andFilters.Add("fi.food_reference_id = @FoodReferenceId");
            parameters.Add("FoodReferenceId", query.FoodReferenceId);
        }
        if (query.FoodGroup != null && query.FoodGroup.Length > 0)
        {
            foreach (FoodGroup item in query.FoodGroup)
            {
                int index = query.FoodGroup.ToList().IndexOf(item);
                orFoodGroupFilters.Add($"fr.food_group = @FoodGroup{index}");
                parameters.Add($"FoodGroup{index}", item.ToString());
            }
        }
        if (query.Status != null && query.Status.Length > 0)
        {
            foreach (FoodItemStatus status in query.Status)
            {
                orStatusFilters.Add(status switch
                {
                    FoodItemStatus.Expired => "fi.expiration_date < CURRENT_DATE",
                    FoodItemStatus.Expiring => "fi.expiration_date >= CURRENT_DATE AND fi.expiration_date <= CURRENT_DATE + INTERVAL '3 days'",
                    FoodItemStatus.Fresh => "fi.expiration_date > CURRENT_DATE + INTERVAL '3 days'",
                    _ => ""
                });
            }
        }
        if (query.FromQuantity.HasValue)
        {
            andFilters.Add("fi.quantity >= @FromQuantity");
            parameters.Add("FromQuantity", query.FromQuantity.Value);
        }
        if (query.ToQuantity.HasValue)
        {
            andFilters.Add("fi.quantity <= @ToQuantity");
            parameters.Add("ToQuantity", query.ToQuantity.Value);
        }
        if (!string.IsNullOrWhiteSpace(query.SearchText))
        {
            andFilters.Add("fi.name ILIKE @SearchText");
            parameters.Add("SearchText", $"%{query.SearchText}%");
        }
        if (query.ExpirationDateAfter.HasValue)
        {
            andFilters.Add("fi.expiration_date > @ExpirationDateAfter"); // use >= for inclusive
            parameters.Add("ExpirationDateAfter", query.ExpirationDateAfter.Value);
        }

        if (query.ExpirationDateBefore.HasValue)
        {
            andFilters.Add("fi.expiration_date < @ExpirationDateBefore"); // use <= for inclusive
            parameters.Add("ExpirationDateBefore", query.ExpirationDateBefore.Value);
        }
        string? andFilterClause = andFilters.Count > 0 ? string.Join(" AND ", andFilters) : null;
        string? orFoodGroupFilterClause = orFoodGroupFilters.Count > 0 ? "(" + string.Join(" OR ", orFoodGroupFilters) + ")" : null;
        string? orStatusFilterClause = orStatusFilters.Count > 0 ? "(" + string.Join(" OR ", orStatusFilters) + ")" : null;

        string whereClause = andFilterClause != null || orFoodGroupFilterClause != null || orStatusFilterClause != null
            ? "WHERE " + string.Join(" AND ", new[] { andFilterClause,  orFoodGroupFilterClause, orStatusFilterClause }.Where(c => c != null))
            : string.Empty;

        string sql = $"""
            SELECT COUNT(*) 
                FROM food_items fi
                LEFT JOIN food_references fr ON fi.food_reference_id = fr.id
                {whereClause};
            SELECT
                fi.id AS {nameof(FoodItemPreviewRow.Id)},
                fi.name AS {nameof(FoodItemPreviewRow.Name)},
                fr.food_group AS {nameof(FoodItemPreviewRow.FoodGroup)},
                fi.image_url AS {nameof(FoodItemPreviewRow.ImageUrl)},
                fi.quantity AS {nameof(FoodItemPreviewRow.Quantity)},
                u.abbreviation AS {nameof(FoodItemPreviewRow.UnitAbbreviation)},
                fi.expiration_date AS {nameof(FoodItemPreviewRow.ExpirationDate)}
            FROM food_items fi
            LEFT JOIN food_references fr ON fi.food_reference_id = fr.id
            LEFT JOIN units u ON fi.unit_id = u.id
            {whereClause}
            {orderClause}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
         """;

        parameters.Add("Offset", (query.PageNumber - 1) * query.PageSize);
        parameters.Add("PageSize", query.PageSize);

        SqlMapper.GridReader multi = await connection.QueryMultipleAsync(sql, parameters);
        int totalCount = await multi.ReadFirstAsync<int>();
        IEnumerable<FoodItemPreviewRow> rows = await multi.ReadAsync<FoodItemPreviewRow>();

        var pagedResponse = PagedList<FoodItemPreview>.Create(
            rows.Select(r => new FoodItemPreview(
                                r.Id,
                                r.Name,
                                r.FoodGroup.ToReadableString(), // <-- apply your extension here
                                r.ImageUrl,
                                r.Quantity,
                                r.UnitAbbreviation,
                                r.ExpirationDate
                            )).ToList(),
            totalCount,
            query.PageNumber,
            query.PageSize);
        return pagedResponse;
    }
}
