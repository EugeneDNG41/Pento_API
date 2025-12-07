using Pento.Domain.Abstractions;

namespace Pento.Domain.Storages;

public sealed class StorageCreatedDomainEvent(Guid storageId, Guid userId) : DomainEvent
{
    public Guid StorageId { get; init; } = storageId;
    public Guid UserId { get; init; } = userId;
}
