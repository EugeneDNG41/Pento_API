namespace Pento.Application.Compartments.Get;

public sealed record CompartmentResponse(Guid Id, Guid StorageId, Guid HouseholdId, string Name, string? Notes);
