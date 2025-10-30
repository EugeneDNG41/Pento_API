
using Pento.Application.Abstractions.Messaging;
namespace Pento.Application.StorageItems.Get;

public sealed record GetStorageItemQuery(Guid Id) : IQuery<StorageItemResponse>;
