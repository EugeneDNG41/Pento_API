namespace Pento.Application.Storages.GetAll;

public sealed record StorageResponse(
    Guid Id,
    Guid HouseholdId,
    string Name,
    string Type,
    string? Notes
);
public sealed record StoragePreview
{
    public Guid StorageId { get; init; }
    public string StorageName { get; init; }
    public string StorageType { get; init; }
    public int TotalCompartments { get; init; }
}
