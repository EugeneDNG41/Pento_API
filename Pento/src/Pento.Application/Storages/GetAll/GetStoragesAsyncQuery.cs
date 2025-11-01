using Pento.Application.Abstractions.Messaging;
using Pento.Application.Storages.Get;

namespace Pento.Application.Storages.GetAll;

public sealed record GetStoragesAsyncQuery : IQuery<IReadOnlyList<StorageResponse>>;
