using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.Storages;

public sealed class StorageTypeChangedDomainEvent(Guid storageId, StorageType newType) : DomainEvent
{
    public Guid StorageId { get; init; } = storageId;
    public StorageType NewType { get; init; } = newType;
}
