namespace Pento.Application.GiveawayPosts.Search;

public sealed record GiveawayPostPreviewRow
{
    public Guid Id { get; init; }
    public Guid FoodItemId { get; init; }
    public Guid FoodRefId { get; init; }
    public string FoodName { get; init; } = "";
    public Uri? FoodImageUrl { get; init; }
    public string TitleDescription { get; init; } = "";
    public string ContactInfo { get; init; } = "";
    public string Address { get; init; } = "";
    public decimal Quantity { get; init; }
    public string Status { get; init; } = "";
    public DateTime? PickupStartDate { get; init; }
    public DateTime? PickupEndDate { get; init; }
    public string PickupOption { get; init; } = "";
    public DateTime CreatedOnUtc { get; init; }
}

public sealed record GiveawayPostPreview(
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
    DateTime CreatedOnUtc
);

