using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Services;
using Pento.Domain.Abstractions;
using Pento.Domain.Activities;
using Pento.Domain.GroceryLists;
using Pento.Domain.UserActivities;

namespace Pento.Application.EventHandlers;

internal sealed class GroceryListCreatedEventHandler(
    IActivityService activityService,
    IMilestoneService milestoneService,
    IGenericRepository<GroceryList> groceryListRepository,
    IUnitOfWork unitOfWork) : DomainEventHandler<GroceryListCreatedDomainEvent>
{
    public async override Task Handle(GroceryListCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        GroceryList? groceryList = await groceryListRepository.GetByIdAsync(domainEvent.GroceryListId, cancellationToken);
        if (groceryList == null)
        {
            throw new PentoException(nameof(GroceryListCreatedEventHandler), GroceryListErrors.NotFound);
        }
        Result<UserActivity> createResult = await activityService.RecordActivityAsync(
            domainEvent.UserId,
            groceryList.HouseholdId,
            ActivityCode.GROCERY_LIST_CREATE.ToString(),
            domainEvent.GroceryListId,
            cancellationToken);
        if (createResult.IsFailure)
        {
            throw new PentoException(nameof(GroceryListCreatedEventHandler), createResult.Error);
        }
        Result milestoneCheckResult = await milestoneService.CheckMilestoneAfterActivityAsync(createResult.Value, cancellationToken);
        if (milestoneCheckResult.IsFailure)
        {
            throw new PentoException(nameof(GroceryListCreatedEventHandler), milestoneCheckResult.Error);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
