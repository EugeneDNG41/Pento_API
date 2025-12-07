using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.Abstractions;

namespace Pento.Application.GiveawayClaims.Search;

internal sealed class SearchGiveawayClaimQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory
) : IQueryHandler<SearchGiveawayClaimQuery, PagedList<GiveawayClaimPreview>>
{
    public async Task<Result<PagedList<GiveawayClaimPreview>>> Handle(
        SearchGiveawayClaimQuery query,
        CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);

        var filters = new List<string> { "gc.is_deleted IS FALSE" };
        var parameters = new DynamicParameters();

        if (query.GiveawayPostId.HasValue)
        {
            filters.Add("gc.giveaway_post_id = @GiveawayPostId");
            parameters.Add("GiveawayPostId", query.GiveawayPostId);
        }

        if (query.Status.HasValue)
        {
            filters.Add("gc.status = @Status");
            parameters.Add("Status", query.Status.ToString());
        }

        if (!string.IsNullOrWhiteSpace(query.SearchText))
        {
            filters.Add("(gc.message ILIKE @GetAll OR fr.name ILIKE @GetAll)");
            parameters.Add("GetAll", $"%{query.SearchText}%");
        }

        string whereClause = filters.Count > 0
            ? "WHERE " + string.Join(" AND ", filters)
            : string.Empty;

        string orderBy = query.SortBy switch
        {
            "post" => $"gc.giveaway_post_id {(query.Descending ? "DESC" : "ASC")}",
            "date" => $"gc.created_on_utc {(query.Descending ? "DESC" : "ASC")}",
            _ => $"gc.created_on_utc DESC" // default sort: newest first
        };

        string sql = $"""
            SELECT COUNT(*)
            FROM giveaway_claims gc
            JOIN giveaway_posts gp ON gc.giveaway_post_id = gp.id
            JOIN food_items fi ON gp.food_item_id = fi.id
            JOIN food_references fr ON fi.food_reference_id = fr.id
            {whereClause};

            SELECT
                gc.id AS {nameof(GiveawayClaimPreviewRow.Id)},
                gc.giveaway_post_id AS {nameof(GiveawayClaimPreviewRow.GiveawayPostId)},
                gc.claimant_id AS {nameof(GiveawayClaimPreviewRow.ClaimantId)},
                gc.message AS {nameof(GiveawayClaimPreviewRow.Message)},
                gc.status AS {nameof(GiveawayClaimPreviewRow.Status)},
                gc.created_on_utc AS {nameof(GiveawayClaimPreviewRow.CreatedOnUtc)},

                fr.id AS {nameof(GiveawayClaimPreviewRow.FoodRefId)},
                fr.name AS {nameof(GiveawayClaimPreviewRow.FoodName)},
                fr.image_url AS {nameof(GiveawayClaimPreviewRow.FoodImageUrl)}

            FROM giveaway_claims gc
            JOIN giveaway_posts gp ON gc.giveaway_post_id = gp.id
            JOIN food_items fi ON gp.food_item_id = fi.id
            JOIN food_references fr ON fi.food_reference_id = fr.id
            {whereClause}
            ORDER BY {orderBy}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        """;

        parameters.Add("Offset", (query.PageNumber - 1) * query.PageSize);
        parameters.Add("PageSize", query.PageSize);

        SqlMapper.GridReader multi = await connection.QueryMultipleAsync(sql, parameters);

        int totalCount = await multi.ReadFirstAsync<int>();
        IEnumerable<GiveawayClaimPreviewRow> rows = await multi.ReadAsync<GiveawayClaimPreviewRow>();

        var items = rows.Select(r => new GiveawayClaimPreview(
            r.Id,
            r.GiveawayPostId,
            r.ClaimantId,
            r.Message,
            r.Status,
            r.CreatedOnUtc,
            r.FoodRefId,
            r.FoodName,
            r.FoodImageUrl
        )).ToList();

        return Result.Success(
            PagedList<GiveawayClaimPreview>.Create(items, totalCount, query.PageNumber, query.PageSize)
        );
    }
}
