using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Domain.Abstractions;
using Pento.Domain.Activities;
using Pento.Domain.Storages;
using Pento.Domain.UserActivities;

namespace Pento.Application.EventHandlers.Storages;

internal sealed class StorageCreatedEventHandler(
    IActivityService activityService,
    IGenericRepository<Storage> storageRepository,
    IUnitOfWork unitOfWork) : DomainEventHandler<StorageCreatedDomainEvent>
{
    public async override Task Handle(StorageCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Storage? storage = await storageRepository.GetByIdAsync(domainEvent.StorageId, cancellationToken);
        if (storage == null)
        {
            throw new PentoException(nameof(StorageCreatedEventHandler), StorageErrors.NotFound);
        }
        Result<UserActivity> createResult = await activityService.RecordActivityAsync(
            domainEvent.UserId,
            storage.HouseholdId,
            ActivityCode.STORAGE_CREATE.ToString(),
            domainEvent.StorageId,
            cancellationToken);
        if (createResult.IsFailure)
        {
            throw new PentoException(nameof(StorageCreatedEventHandler), createResult.Error);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
