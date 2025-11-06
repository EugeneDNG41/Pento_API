using Marten.Pagination;
using Pento.Domain.FoodItems.Projections;

namespace Pento.Application.Compartments.Get;

public sealed record CompartmentWithFoodItemPreviewResponse(Guid Id, Guid StorageId, Guid HouseholdId, string Name, string? Notes, IPagedList<FoodItemPreview> previews);
