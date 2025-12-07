using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.Abstractions;

namespace Pento.Application.GiveawayPosts.Search;

internal sealed class SearchGiveawayPostQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory
) : IQueryHandler<SearchGiveawayPostQuery, PagedList<GiveawayPostPreview>>
{
    public async Task<Result<PagedList<GiveawayPostPreview>>> Handle(
        SearchGiveawayPostQuery query,
        CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);

        var filters = new List<string>
        {
            "gp.is_deleted IS FALSE"
        };

        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(query.SearchText))
        {
            filters.Add("(fr.name ILIKE @SearchText OR gp.title_description ILIKE @SearchText)");
            parameters.Add("SearchText", $"%{query.SearchText}%");
        }

        if (query.Status.HasValue)
        {
            filters.Add("gp.status = @Status");
            parameters.Add("Status", query.Status.ToString());
        }

        if (query.PickupOption.HasValue)
        {
            filters.Add("gp.pickup_option = @PickupOption");
            parameters.Add("PickupOption", query.PickupOption.ToString());
        }

        string whereClause = filters.Count > 0
            ? "WHERE " + string.Join(" AND ", filters)
            : string.Empty;

        string sql = $"""
            SELECT COUNT(*)
            FROM giveaway_posts gp
            JOIN food_items fi ON gp.food_item_id = fi.id
            JOIN food_references fr ON fi.food_reference_id = fr.id
            {whereClause};

            SELECT 
                gp.id AS {nameof(GiveawayPostPreviewRow.Id)},
                gp.food_item_id AS {nameof(GiveawayPostPreviewRow.FoodItemId)},
                fr.id AS {nameof(GiveawayPostPreviewRow.FoodRefId)},
                fr.name AS {nameof(GiveawayPostPreviewRow.FoodName)},
                fr.image_url AS {nameof(GiveawayPostPreviewRow.FoodImageUrl)},
                gp.title_description AS {nameof(GiveawayPostPreviewRow.TitleDescription)},
                gp.contact_info AS {nameof(GiveawayPostPreviewRow.ContactInfo)},
                gp.address AS {nameof(GiveawayPostPreviewRow.Address)},
                gp.quantity AS {nameof(GiveawayPostPreviewRow.Quantity)},
                gp.status AS {nameof(GiveawayPostPreviewRow.Status)},
                gp.pickup_start_date AS {nameof(GiveawayPostPreviewRow.PickupStartDate)},
                gp.pickup_end_date AS {nameof(GiveawayPostPreviewRow.PickupEndDate)},
                gp.pickup_option AS {nameof(GiveawayPostPreviewRow.PickupOption)},
                gp.created_on_utc AS {nameof(GiveawayPostPreviewRow.CreatedOnUtc)}
            FROM giveaway_posts gp
            JOIN food_items fi ON gp.food_item_id = fi.id
            JOIN food_references fr ON fi.food_reference_id = fr.id
            {whereClause}
            ORDER BY gp.created_on_utc DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        """;

        parameters.Add("Offset", (query.PageNumber - 1) * query.PageSize);
        parameters.Add("PageSize", query.PageSize);

        SqlMapper.GridReader multi = await connection.QueryMultipleAsync(sql, parameters);

        int totalCount = await multi.ReadFirstAsync<int>();
        IEnumerable<GiveawayPostPreviewRow> rows = await multi.ReadAsync<GiveawayPostPreviewRow>();

        var items = rows
            .Select(r => new GiveawayPostPreview(
                r.Id,
                r.FoodItemId,
                r.FoodRefId,
                r.FoodName,
                r.FoodImageUrl,
                r.TitleDescription,
                r.ContactInfo,
                r.Address,
                r.Quantity,
                r.Status,
                r.PickupStartDate,
                r.PickupEndDate,
                r.PickupOption,
                r.CreatedOnUtc
            ))
            .ToList();

        var pagedList = PagedList<GiveawayPostPreview>.Create(
            items,
            totalCount,
            query.PageNumber,
            query.PageSize
        );

        return Result.Success(pagedList);
    }
}
