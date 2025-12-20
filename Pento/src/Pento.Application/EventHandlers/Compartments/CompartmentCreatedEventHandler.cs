using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Domain.Abstractions;
using Pento.Domain.Activities;
using Pento.Domain.Compartments;
using Pento.Domain.UserActivities;

namespace Pento.Application.EventHandlers.Compartments;

internal sealed class CompartmentCreatedEventHandler(
    IActivityService activityService,
    IMilestoneService milestoneService,
    IGenericRepository<Compartment> compartmentRepository,
    IUnitOfWork unitOfWork) : DomainEventHandler<CompartmentCreatedDomainEvent>
{
    public async override Task Handle(CompartmentCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Compartment? compartment = await compartmentRepository.GetByIdAsync(domainEvent.CompartmentId, cancellationToken);
        if (compartment == null)
        {
            throw new PentoException(nameof(CompartmentCreatedEventHandler), CompartmentErrors.NotFound);
        }
        Result<UserActivity> createResult = await activityService.RecordActivityAsync(
            domainEvent.UserId,
            compartment.HouseholdId,
            ActivityCode.COMPARTMENT_CREATE.ToString(),
            domainEvent.CompartmentId,
            cancellationToken);
        if (createResult.IsFailure)
        {
            throw new PentoException(nameof(CompartmentCreatedEventHandler), createResult.Error);
        }
        Result milestoneCheckResult = await milestoneService.CheckMilestoneAfterActivityAsync(createResult.Value, cancellationToken);
        if (milestoneCheckResult.IsFailure)
        {
            throw new PentoException(nameof(CompartmentCreatedEventHandler), milestoneCheckResult.Error);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
