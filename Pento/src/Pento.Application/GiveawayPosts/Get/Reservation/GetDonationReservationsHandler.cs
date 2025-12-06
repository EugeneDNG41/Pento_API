using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.GiveawayPosts.Get.Reservation;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
namespace Pento.Application.GiveawayPosts.Get.Reservation;
internal sealed class GetDonationReservationsHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    IUserContext userContext
) : IQueryHandler<GetDonationReservationsQuery, PagedList<DonationReservationResponse>>
{
    public async Task<Result<PagedList<DonationReservationResponse>>> Handle(
        GetDonationReservationsQuery query,
        CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<PagedList<DonationReservationResponse>>(HouseholdErrors.NotInAnyHouseHold);
        }

        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);

        var filters = new List<string>
        {
            "fir.household_id = @HouseholdId",
            "fir.reservation_for = 'Donation'"
        };

        var parameters = new DynamicParameters();
        parameters.Add("HouseholdId", householdId);

        if (query.GiveawayPostId is not null)
        {
            filters.Add("gp.id = @GiveawayPostId");
            parameters.Add("GiveawayPostId", query.GiveawayPostId);
        }

        if (query.FoodReferenceId is not null)
        {
            filters.Add("fr.id = @FoodReferenceId");
            parameters.Add("FoodReferenceId", query.FoodReferenceId);
        }

        if (query.Status is not null)
        {
            filters.Add("fir.status = @Status");
            parameters.Add("Status", query.Status.ToString());
        }

        string where = "WHERE " + string.Join(" AND ", filters);

        string countSql = $"""
            SELECT COUNT(*)
            FROM food_item_reservations fir
            JOIN giveaway_posts gp ON gp.id = fir.giveaway_post_id
            JOIN food_items fi ON fi.id = fir.food_item_id
            JOIN food_references fr ON fr.id = fi.food_reference_id
            {where};
        """;

        string dataSql = $"""
            SELECT
                fir.id                  AS Id,
                gp.id                   AS GiveawayPostId,
                gp.title_description    AS TitleDescription,
                gp.contact_info         AS ContactInfo,
                gp.address              AS Address,

                fi.id                   AS FoodItemId,
                fr.id                   AS FoodReferenceId,
                fr.name                 AS FoodReferenceName,
                fr.image_url            AS FoodReferenceImageUrl,
                fr.food_group           AS FoodGroup,

                fir.quantity            AS Quantity,
                u.abbreviation          AS UnitAbbreviation,

                fir.reservation_date_utc AS ReservationDateUtc,
                fir.status              AS Status

            FROM food_item_reservations fir
            JOIN giveaway_posts gp ON gp.id = fir.giveaway_post_id
            JOIN food_items fi ON fi.id = fir.food_item_id
            JOIN food_references fr ON fr.id = fi.food_reference_id
            JOIN units u ON u.id = fir.unit_id
            {where}
            ORDER BY fir.reservation_date_utc DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        """;

        parameters.Add("Offset", (query.PageNumber - 1) * query.PageSize);
        parameters.Add("PageSize", query.PageSize);

        int totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

        var rows = (await connection.QueryAsync<DonationReservationResponse>(
            dataSql,
            parameters
        )).ToList();

        return Result.Success(
            PagedList<DonationReservationResponse>.Create(
                rows,
                totalCount,
                query.PageNumber,
                query.PageSize
            )
        );
    }
}
