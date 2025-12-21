using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Application.EventHandlers.Groceries;
using Pento.Domain.Abstractions;
using Pento.Domain.Activities;
using Pento.Domain.Households;
using Pento.Domain.RecipeWishLists;
using Pento.Domain.UserActivities;

namespace Pento.Application.EventHandlers.Households;

internal sealed class HouseholdCreatedEventHandler(
    IActivityService activityService,
    IMilestoneService milestoneService,
    IGenericRepository<RecipeWishList> recipeWishListRepository,
    IGenericRepository<Household> householdRepository,
    IUnitOfWork unitOfWork) : DomainEventHandler<HouseholdCreatedDomainEvent>
{
    public async override Task Handle(HouseholdCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Household? household = await householdRepository.GetByIdAsync(domainEvent.HouseholdId, cancellationToken);
        if (household == null)
        {
            throw new PentoException(nameof(HouseholdCreatedEventHandler), HouseholdErrors.NotFound);
        }
        IEnumerable<RecipeWishList> wishList = await recipeWishListRepository.FindAsync(rw => rw.UserId == domainEvent.UserId,
            cancellationToken: cancellationToken);
        foreach (RecipeWishList wish in wishList)
        {
            wish.SetHouseholdId(domainEvent.HouseholdId);
        }
        await recipeWishListRepository.UpdateRangeAsync(wishList, cancellationToken);

        Result<UserActivity> createResult = await activityService.RecordActivityAsync(
            domainEvent.UserId,
            domainEvent.HouseholdId,
            ActivityCode.HOUSEHOLD_CREATE.ToString(),
            domainEvent.HouseholdId,
            cancellationToken);
        if (createResult.IsFailure)
        {
            throw new PentoException(nameof(HouseholdCreatedEventHandler), createResult.Error);
        }
        Result milestoneCheckResult = await milestoneService.CheckMilestoneAfterActivityAsync(createResult.Value, cancellationToken);
        if (milestoneCheckResult.IsFailure)
        {
            throw new PentoException(nameof(GroceryListCreatedEventHandler), milestoneCheckResult.Error);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
