namespace Pento.Application.Storages.GetById;

public sealed record StorageResponse(
    Guid Id,
    Guid HouseholdId,
    string Name,
    string Type,
    string? Notes
);
