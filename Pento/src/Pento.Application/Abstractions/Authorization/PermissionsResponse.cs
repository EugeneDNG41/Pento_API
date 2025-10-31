namespace Pento.Application.Abstractions.Authorization;

public sealed record PermissionsResponse(Guid UserId, Guid? HouseholdId, HashSet<string> Roles);
public sealed record RolesResponse(Guid UserId, Guid? HouseholdId, HashSet<string> Roles);
public sealed record RoleResponse(string Role);
