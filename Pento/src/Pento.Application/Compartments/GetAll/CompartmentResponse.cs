namespace Pento.Application.Compartments.GetAll;

public sealed record CompartmentResponse(Guid Id, Guid StorageId, Guid HouseholdId, string Name, string? Notes);
