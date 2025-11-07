
using JasperFx.Events;
using Marten;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItems.Projections;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.Households;
using Pento.Domain.Users;


namespace Pento.Application.FoodItems.GetById;

internal sealed class GetFoodItemByIdQueryHandler(
    IUserContext userContext,
    IQuerySession querySession)
    : IQueryHandler<GetFoodItemByIdQuery, FoodItemDetail>
{
    public async Task<Result<FoodItemDetail>> Handle(GetFoodItemByIdQuery request, CancellationToken cancellationToken)
    {
        Guid? currentHouseholdId = userContext.HouseholdId;
        if (currentHouseholdId == Guid.Empty)
        {
            return Result.Failure<FoodItemDetail>(HouseholdErrors.NotInAnyHouseHold);
        }
        FoodItemDetail? foodItem = await querySession.LoadAsync<FoodItemDetail>(request.Id, cancellationToken);
        if (foodItem is null)
        {
           return Result.Failure<FoodItemDetail>(FoodItemErrors.NotFound);
        }
        bool belongsToCurrentHousehold = await querySession
            .Query<FoodItem>()
            .AnyAsync(fi => fi.Id == request.Id && fi.HouseholdId == currentHouseholdId, cancellationToken);
        if (!belongsToCurrentHousehold)
        {
            return Result.Failure<FoodItemDetail>(FoodItemErrors.ForbiddenAccess);
        }
        return foodItem;
    }
}
