using Pento.Application.Abstractions.Pagination;
using Pento.Application.Compartments.GetAll;

namespace Pento.Application.Storages.GetById;

public sealed record StorageDetailResponse(
    Guid Id,
    Guid HouseholdId,
    string Name,
    string Type,
    string? Notes,
    PagedList<CompartmentPreview> Compartments
);
