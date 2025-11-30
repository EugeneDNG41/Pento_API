using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;

namespace Pento.Application.GiveawayPosts.Get;

internal sealed class GetGiveawayPostByIdQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory
) : IQueryHandler<GetGiveawayPostByIdQuery, GiveawayPostDetailResponse>
{
    public async Task<Result<GiveawayPostDetailResponse>> Handle(
        GetGiveawayPostByIdQuery query,
        CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);

        string sql = $"""
            SELECT 
                gp.id AS {nameof(GiveawayPostDetailRow.Id)},
                gp.food_item_id AS {nameof(GiveawayPostDetailRow.FoodItemId)},
                fr.id AS {nameof(GiveawayPostDetailRow.FoodRefId)},
                fr.name AS {nameof(GiveawayPostDetailRow.FoodName)},
                fr.image_url AS {nameof(GiveawayPostDetailRow.FoodImageUrl)},
                gp.title_description AS {nameof(GiveawayPostDetailRow.TitleDescription)},
                gp.contact_info AS {nameof(GiveawayPostDetailRow.ContactInfo)},
                gp.address AS {nameof(GiveawayPostDetailRow.Address)},
                gp.quantity AS {nameof(GiveawayPostDetailRow.Quantity)},
                gp.status AS {nameof(GiveawayPostDetailRow.Status)},
                gp.pickup_start_date AS {nameof(GiveawayPostDetailRow.PickupStartDate)},
                gp.pickup_end_date AS {nameof(GiveawayPostDetailRow.PickupEndDate)},
                gp.pickup_option AS {nameof(GiveawayPostDetailRow.PickupOption)},
                gp.created_on_utc AS {nameof(GiveawayPostDetailRow.CreatedOnUtc)},
                gp.updated_on_utc AS {nameof(GiveawayPostDetailRow.UpdatedOnUtc)}
            FROM giveaway_posts gp
            JOIN food_items fi ON gp.food_item_id = fi.id
            JOIN food_references fr ON fi.food_reference_id = fr.id
            WHERE gp.id = @Id AND gp.is_deleted IS FALSE
            LIMIT 1;
        """;

        GiveawayPostDetailRow? row = await connection.QueryFirstOrDefaultAsync<GiveawayPostDetailRow>(
            sql, new { query.Id });

        if (row is null)
        {
            return Result.Failure<GiveawayPostDetailResponse>(
                Error.NotFound("GIVEAWAY.NOT_FOUND", "Giveaway post not found.")
            );
        }

        var response = new GiveawayPostDetailResponse(
            row.Id,
            row.FoodItemId,
            row.FoodRefId,
            row.FoodName,
            row.FoodImageUrl,
            row.TitleDescription,
            row.ContactInfo,
            row.Address,
            row.Quantity,
            row.Status,
            row.PickupStartDate,
            row.PickupEndDate,
            row.PickupOption,
            row.CreatedOnUtc,
            row.UpdatedOnUtc
        );

        return Result.Success(response);
    }
}
