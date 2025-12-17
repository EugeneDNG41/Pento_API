using Pento.Application.Users.GetAll;

namespace Pento.Application.Trades.Requests.AdminGetAll;

public sealed record TradeOfferAdminResponse
{
    public Guid TradeOfferId { get; init; }
    public BasicUserResponse OfferUser { get; init; }
    public string OfferHouseholdName { get; init; }
    public string Status { get; init; }
    public DateTime CreatedOn { get; init; }
    public DateTime UpdatedOn { get; init; }
    public int TotalItems { get; init; }
    public int TotalRequests { get; init; }
    public bool IsDeleted { get; init; }
}
public sealed record TradeOfferAdminRow
{
    public Guid TradeOfferId { get; init; }
    public Guid UserId { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public Uri? AvatarUrl { get; init; }
    public string OfferHouseholdName { get; init; }
    public string Status { get; init; }
    public DateTime CreatedOn { get; init; }
    public DateTime UpdatedOn { get; init; }
    public int TotalItems { get; init; }
    public int TotalRequests { get; init; }
    public bool IsDeleted { get; init; }
}
