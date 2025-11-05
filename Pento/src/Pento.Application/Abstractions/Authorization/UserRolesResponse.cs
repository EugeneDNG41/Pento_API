namespace Pento.Application.Abstractions.Authorization;
public sealed record UserRolesResponse(Guid UserId, Guid? HouseholdId, HashSet<string> Roles);
