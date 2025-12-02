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
        var filters = new List<string>
        {
            "fi.is_deleted IS FALSE",
        };
        var parameters = new DynamicParameters();
        if (query.FoodReferenceId  != null)
        {
            filters.Add("fi.food_reference_id = @FoodReferenceId");
            parameters.Add("FoodReferenceId", query.FoodReferenceId);
        }
        if (query.FoodGroup.HasValue)
        {
            filters.Add("food_group = @FoodGroup");
            parameters.Add("FoodGroup", query.FoodGroup.ToString());
        }
        // Quantity range
        if (query.FromQuantity.HasValue)
        {
            filters.Add("quantity >= @FromQuantity");
            parameters.Add("FromQuantity", query.FromQuantity.Value);
        }

        if (query.ToQuantity.HasValue)
        {
            filters.Add("quantity <= @ToQuantity");
            parameters.Add("ToQuantity", query.ToQuantity.Value);
        }
        if (!string.IsNullOrWhiteSpace(query.SearchText))
        {
            filters.Add("name ILIKE @SearchText");
            parameters.Add("SearchText", $"%{query.SearchText}%");
        }
        if (query.ExpirationDateAfter.HasValue)
        {
            filters.Add("expiration_date > @ExpirationDateAfter"); // use >= for inclusive
            parameters.Add("ExpirationDateAfter", query.ExpirationDateAfter.Value);
        }

        if (query.ExpirationDateBefore.HasValue)
        {
            filters.Add("expiration_date < @ExpirationDateBefore"); // use <= for inclusive
            parameters.Add("ExpirationDateBefore", query.ExpirationDateBefore.Value);
        }
        string whereClause = filters.Count > 0 ? "WHERE " + string.Join(" AND ", filters) : string.Empty;

        string sql = $"""
            SELECT COUNT(*) 
                FROM food_items fi
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
