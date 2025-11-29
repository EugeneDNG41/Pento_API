namespace Pento.Application.Giveaways.Claims.Get;

public sealed record GiveawayClaimDetailResponse(
    Guid Id,
    Guid GiveawayPostId,
    Guid ClaimantId,
    string? Message,
    string Status,
    DateTime CreatedOnUtc,

    Guid FoodRefId,
    string FoodName,
    Uri? FoodImageUrl
);
public sealed record GiveawayClaimDetailRow
{
    public Guid Id { get; init; }
    public Guid GiveawayPostId { get; init; }
    public Guid ClaimantId { get; init; }
    public string? Message { get; init; }
    public string Status { get; init; } = "";
    public DateTime CreatedOnUtc { get; init; }

    public Guid FoodRefId { get; init; }
    public string FoodName { get; init; } = "";
    public Uri? FoodImageUrl { get; init; }
}

