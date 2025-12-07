using Pento.Domain.FoodItemReservations;

namespace Pento.Application.GiveawayPosts.Get.Reservation;

public sealed record DonationReservationResponse(
    Guid Id,
    Guid GiveawayPostId,
    string TitleDescription,
    string ContactInfo,
    string Address,

    Guid FoodItemId,
    Guid FoodReferenceId,
    string FoodReferenceName,
    Uri? FoodReferenceImageUrl,
    string FoodGroup,

    decimal Quantity,
    string UnitAbbreviation,

    DateTime ReservationDateUtc,
    ReservationStatus Status
);

