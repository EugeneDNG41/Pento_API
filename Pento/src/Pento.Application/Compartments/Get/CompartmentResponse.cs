using Pento.Application.Abstractions.Pagination;
using Pento.Application.FoodItems.Search;

namespace Pento.Application.Compartments.Get;

public sealed record CompartmentResponse(Guid Id, Guid StorageId, Guid HouseholdId, string Name, string? Notes);
public sealed record CompartmentWithFoodItemPreviewResponse(Guid Id, Guid StorageId, Guid HouseholdId, string Name, string? Notes, PagedList<FoodItemPreview> previews);
