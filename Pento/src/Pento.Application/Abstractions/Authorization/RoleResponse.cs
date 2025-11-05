namespace Pento.Application.Abstractions.Authorization;

public sealed class RoleResponse
{
    public string Role { get; init; }
    public List<PermissionResponse> Permissions { get; init; } = new();
}
public sealed class PermissionResponse
{
    public string Permission { get; init; }
    public string Description { get; init; }
}

