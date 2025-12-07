using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.Storages;

namespace Pento.Application.Storages.GetAll;

public sealed record GetStoragesQuery(string? SearchText, StorageType? StorageType, int PageNumber, int PageSize) : IQuery<PagedList<StoragePreview>>;
