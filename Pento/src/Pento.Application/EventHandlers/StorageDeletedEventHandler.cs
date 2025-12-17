using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Compartments;
using Pento.Domain.Storages;

namespace Pento.Application.EventHandlers;

internal sealed class StorageDeletedEventHandler(
    IGenericRepository<Compartment> compartmentRepository,
    IUnitOfWork unitOfWork) : DomainEventHandler<StorageDeletedDomainEvent>
{
    public async override Task Handle(StorageDeletedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        IEnumerable<Compartment> compartments = await compartmentRepository.FindAsync(c => c.StorageId == domainEvent.StorageId, cancellationToken);
        await compartmentRepository.RemoveRangeAsync(compartments, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
