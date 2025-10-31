namespace Pento.Application.Storages.Get;

public sealed record StorageResponse(
    Guid Id,
    Guid HouseholdId,
    string Name,
    string Type,
    string? Notes
);
