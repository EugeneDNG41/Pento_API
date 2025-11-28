namespace Pento.Application.Giveaways.Get;

public sealed record GiveawayPostDetailResponse(
    Guid Id,
    Guid FoodItemId,
    Guid FoodRefId,
    string FoodName,
    Uri? FoodImageUrl,
    string TitleDescription,
    string ContactInfo,
    string Address,
    decimal Quantity,
    string Status,
    DateTime? PickupStartDate,
    DateTime? PickupEndDate,
    string PickupOption,
    DateTime CreatedOnUtc,
    DateTime UpdatedOnUtc
);
