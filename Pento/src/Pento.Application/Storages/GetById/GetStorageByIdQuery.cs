using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Storages.GetById;

public sealed record GetStorageByIdQuery(
    Guid StorageId,
    int PageNumber,
    int PageSize) : IQuery<StorageDetailResponse>;
