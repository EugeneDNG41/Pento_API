namespace Pento.Application.Households.GetCurrent;

public sealed class HouseholdResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string? InviteCode { get; init; }
    public DateTime? InviteCodeExpirationUtc { get; init; }
    public List<HouseholdMemberResponse> Members { get; init; } = new();
}
public sealed class HouseholdMemberResponse
{
    public Guid UserId { get; init; }
    public string Email { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string? AvatarUrl { get; init; }
    public string Roles { get; init; }
}

