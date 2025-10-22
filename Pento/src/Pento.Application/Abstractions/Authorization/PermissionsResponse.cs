namespace Pento.Application.Abstractions.Authorization;

public sealed record PermissionsResponse(Guid UserId, Guid HouseholdId, HashSet<string> Roles);
