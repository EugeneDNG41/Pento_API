namespace Pento.Application.Households.GetCurrent;

public sealed record HouseholdDetailResponse(Guid Id, string Name, string? InviteCode, DateTime? InviteCodeExpirationUtc, IReadOnlyList<HouseholdMember> Members);
public sealed record HouseholdMember(Guid UserId, string Email, string FirstName, string LastName, Uri? AvatarUrl, string Roles);
