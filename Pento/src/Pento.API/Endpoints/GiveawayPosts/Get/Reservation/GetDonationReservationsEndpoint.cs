using Pento.API.Endpoints;
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.GiveawayPosts.Get.Reservation;
using Pento.Domain.FoodItemReservations;

namespace Pento.API.Endpoints.GiveawayPosts.Get.Reservation;
internal sealed class GetDonationReservationsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("trade-reservations", async (
            Guid? giveawayPostId,
            Guid? foodReferenceId,
            ReservationStatus? status,
            int pageNumber,
            int pageSize,
            IQueryHandler<GetDonationReservationsQuery, PagedList<DonationReservationResponse>> handler,
            CancellationToken ct
        ) =>
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var query = new GetDonationReservationsQuery(
                GiveawayPostId: giveawayPostId,
                FoodReferenceId: foodReferenceId,
                Status: status,
                PageNumber: pageNumber,
                PageSize: pageSize
            );

            Pento.Domain.Abstractions.Result<PagedList<DonationReservationResponse>> result = await handler.Handle(query, ct);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Trades)
        .RequireAuthorization();
    }
}
