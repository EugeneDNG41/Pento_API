using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.FoodItemReservations;

namespace Pento.Application.GiveawayPosts.Get.Reservation;

public sealed record GetDonationReservationsQuery(
    Guid? GiveawayPostId,
    Guid? FoodReferenceId,
    ReservationStatus? Status,
    int PageNumber,
    int PageSize
) : IQuery<PagedList<DonationReservationResponse>>;

