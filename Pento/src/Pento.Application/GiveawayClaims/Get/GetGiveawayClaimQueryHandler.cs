using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;

namespace Pento.Application.GiveawayClaims.Get;

internal sealed class GetGiveawayClaimByIdQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory
) : IQueryHandler<GetGiveawayClaimQuery, GiveawayClaimDetailResponse>
{
    public async Task<Result<GiveawayClaimDetailResponse>> Handle(
        GetGiveawayClaimQuery query,
        CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);

        string sql = $"""
            SELECT
                gc.id AS {nameof(GiveawayClaimDetailRow.Id)},
                gc.giveaway_post_id AS {nameof(GiveawayClaimDetailRow.GiveawayPostId)},
                gc.claimant_id AS {nameof(GiveawayClaimDetailRow.ClaimantId)},
                gc.message AS {nameof(GiveawayClaimDetailRow.Message)},
                gc.status AS {nameof(GiveawayClaimDetailRow.Status)},
                gc.created_on_utc AS {nameof(GiveawayClaimDetailRow.CreatedOnUtc)},

                fr.id AS {nameof(GiveawayClaimDetailRow.FoodRefId)},
                fr.name AS {nameof(GiveawayClaimDetailRow.FoodName)},
                fr.image_url AS {nameof(GiveawayClaimDetailRow.FoodImageUrl)}

            FROM giveaway_claims gc
            JOIN giveaway_posts gp ON gc.giveaway_post_id = gp.id
            JOIN food_items fi ON gp.food_item_id = fi.id
            JOIN food_references fr ON fi.food_reference_id = fr.id
            WHERE gc.id = @Id AND gc.is_deleted IS FALSE
            LIMIT 1;
        """;

        GiveawayClaimDetailRow? row = await connection.QueryFirstOrDefaultAsync<GiveawayClaimDetailRow>(sql, new { query.ClaimId });

        if (row is null)
        {
            return Result.Failure<GiveawayClaimDetailResponse>(
                Error.NotFound("CLAIM.NOT_FOUND", "Giveaway claim not found.")
            );
        }

        var response = new GiveawayClaimDetailResponse(
            row.Id,
            row.GiveawayPostId,
            row.ClaimantId,
            row.Message,
            row.Status,
            row.CreatedOnUtc,
            row.FoodRefId,
            row.FoodName,
            row.FoodImageUrl
        );

        return Result.Success(response);
    }
}
