namespace Pento.Application.Abstractions.Authorization;

public sealed record UserPermissionsResponse(Guid UserId, Guid? HouseholdId, HashSet<string> Permissions, bool IsDeleted);
