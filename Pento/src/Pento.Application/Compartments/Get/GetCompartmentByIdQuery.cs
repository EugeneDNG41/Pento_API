using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Compartments.Get;

public sealed record GetCompartmentByIdQuery(
    Guid CompartmentId,
    string? SearchText,
    int PageNumber,
    int PageSize) : IQuery<CompartmentWithFoodItemPreviewResponse>;
