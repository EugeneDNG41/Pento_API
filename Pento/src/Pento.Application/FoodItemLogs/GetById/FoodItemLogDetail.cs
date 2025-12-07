using Pento.Application.Users.GetAll;

namespace Pento.Application.FoodItemLogs.GetById;

public sealed record FoodItemLogDetail(
    Guid Id,
    Guid FoodItemId,
    string FoodItemName,
    Uri? FoodItemImageUrl,
    BasicUserResponse User,
    DateTime Timestamp,
    string Action,
    decimal Quantity,
    string UnitAbbreviation);
public sealed record FoodItemLogDetailRow
{
    public Guid Id { get; init; }
    public Guid FoodItemId { get; init; }
    public string FoodItemName { get; init; }
    public Uri? FoodItemImageUrl { get; init; }
    public Guid UserId { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public Uri? AvatarUrl { get; init; }
    public DateTime Timestamp { get; init; }
    public string Action { get; init; }
    public decimal Quantity { get; init; }
    public string UnitAbbreviation { get; init; }
}
