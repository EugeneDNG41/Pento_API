using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.GroceryLists;
using Pento.Domain.GroceryListItems;
using Pento.Domain.GroceryListAssignees;
using Pento.Application.Abstractions.UtilityServices.Clock;

namespace Pento.Application.GroceryLists.Create;

internal sealed class CreateGroceryListDetailCommandHandler(
    IGenericRepository<GroceryList> groceryListRepository,
    IGenericRepository<GroceryListItem> groceryListItemRepository,
    IUserContext userContext,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<CreateGroceryListDetailCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateGroceryListDetailCommand request, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<Guid>(GroceryListErrors.ForbiddenAccess);
        }

        bool exists = await groceryListRepository.AnyAsync(
            x => x.HouseholdId == householdId && x.Name == request.Name,
            cancellationToken
        );

        if (exists)
        {
            return Result.Failure<Guid>(GroceryListErrors.DuplicateName);
        }

        DateTime utcNow = dateTimeProvider.UtcNow;

        var groceryList = GroceryList.Create(
            householdId: householdId.Value,
            name: request.Name,
            createdBy: userContext.UserId,
            createdOnUtc: utcNow, userContext.UserId
        );

        groceryListRepository.Add(groceryList);

        foreach (GroceryListItemRequest item in request.Items)
        {
            if (!Enum.TryParse<GroceryItemPriority>(item.Priority, true, out GroceryItemPriority priority))
            {
                priority = GroceryItemPriority.Medium;
            }

            var groceryItem = new GroceryListItem(
                id: Guid.CreateVersion7(),
                listId: groceryList.Id,
                foodRefId: item.FoodRefId ?? Guid.Empty,
                quantity: item.Quantity,
                addedBy: userContext.UserId,
                createdOnUtc: utcNow,
                customName: item.CustomName,
                unitId: item.UnitId,
                notes: item.Notes,
                priority: priority
            );

            groceryListItemRepository.Add(groceryItem);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return groceryList.Id;
    }
}

