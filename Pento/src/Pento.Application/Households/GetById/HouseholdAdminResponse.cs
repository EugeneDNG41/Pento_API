namespace Pento.Application.Households.GetById;

public sealed class HouseholdAdminResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string? InviteCode { get; init; }
    public DateTime? InviteCodeExpirationUtc { get; init; }
    public bool IsDeleted { get; init; }
    public List<HouseholdMemberAdminResponse> Members { get; init; } = new();

}
public sealed class HouseholdMemberAdminResponse
{
    public Guid UserId { get; init; }
    public string Email { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string? AvatarUrl { get; init; }
    public string Roles { get; init; }
    public bool IsDeleted { get; init; }
}
