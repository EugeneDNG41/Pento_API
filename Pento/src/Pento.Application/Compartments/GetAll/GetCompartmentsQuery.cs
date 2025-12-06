using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;

namespace Pento.Application.Compartments.GetAll;

public sealed record GetCompartmentsQuery(
    Guid StorageId,
    string? SearchText,
    int PageNumber,
    int PageSize) : IQuery<PagedList<CompartmentPreview>>;
