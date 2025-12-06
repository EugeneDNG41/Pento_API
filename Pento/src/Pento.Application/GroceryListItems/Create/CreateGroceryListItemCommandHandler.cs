using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;
using Pento.Domain.GroceryListItems;
using Pento.Domain.GroceryLists;
using Pento.Domain.Units;

namespace Pento.Application.GroceryListItems.Create;

internal sealed class CreateGroceryListItemCommandHandler(
    IGenericRepository<GroceryListItem> _groceryListItemRepository,
    IGenericRepository<FoodReference> _foodrefRepo,
    IGenericRepository<GroceryList> _grocerylistRepository,
    IGenericRepository<Unit> _unitrepository,
    IUserContext userContext,
    IUnitOfWork _unitOfWork,
    IDateTimeProvider _dateTimeProvider
) : ICommandHandler<CreateGroceryListItemCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateGroceryListItemCommand request, CancellationToken cancellationToken)
    {
        if (userContext.UserId == Guid.Empty)
        {
            return Result.Failure<Guid>(GroceryListItemErrors.ForbiddenAccess);
        }
        GroceryList groceryList = await _grocerylistRepository.GetByIdAsync(request.ListId, cancellationToken);
        if (groceryList is null)
        {
            return Result.Failure<Guid>(GroceryListErrors.NotFound);
        }
        FoodReference foodReference = await _foodrefRepo.GetByIdAsync(request.FoodRefId, cancellationToken);
        if (foodReference is null)
        {
            return Result.Failure<Guid>(FoodReferenceErrors.NotFound);
        }
        Unit unit = await _unitrepository.GetByIdAsync(request.UnitId ?? Guid.Empty, cancellationToken);
        if (unit is null)
        {
            return Result.Failure<Guid>(UnitErrors.NotFound);
        }
        if (!Enum.TryParse<GroceryItemPriority>(request.Priority, true, out GroceryItemPriority priority))
        {
            return Result.Failure<Guid>(GroceryListItemErrors.InvalidPriority);
        }
        if (userContext.UserId == Guid.Empty)
        {
            return Result.Failure<Guid>(GroceryListItemErrors.ForbiddenAccess);
        }

        var groceryListItem = new GroceryListItem(
            id: Guid.NewGuid(),
            listId: request.ListId,
            foodRefId: request.FoodRefId,
            quantity: request.Quantity,
            addedBy: userContext.UserId,
            createdOnUtc: _dateTimeProvider.UtcNow,
            customName: request.CustomName,
            unitId: request.UnitId,
            notes: request.Notes,
            priority: priority
        );

        _groceryListItemRepository.Add(groceryListItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return groceryListItem.Id;
    }
}
