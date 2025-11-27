namespace Pento.Application.Users.Get;

public sealed record UserResponseRow
{
    public Guid Id { get; init; }
    public Guid? HouseholdId { get; init; }
    public string? HouseholdName { get; init; }
    public Uri? AvatarUrl { get; init; }
    public string Email { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public DateTime CreatedAt { get; init; }
    public string Roles { get; init; }
}
public sealed record UserResponse(
    Guid Id,
    Guid? HouseholdId,
    string? HouseholdName,
    Uri? AvatarUrl,
    string Email, 
    string FirstName, 
    string LastName,
    DateTime CreatedAt,
    string Roles,
    IReadOnlyList<UserSubscriptionPreview> ActiveSubscriptions);

public sealed record UserSubscriptionPreview
{
    public Guid UserSubscriptionId { get; init; }
    public string SubscriptionName { get; init; }
    public string Duration { get; init; }
}
