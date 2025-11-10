
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
#pragma warning disable S1481
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
        FoodItem? foodItem = await querySession.Query<FoodItem>().Where(f => f.HouseholdId == currentHouseholdId && f.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
        FoodItemDetail? foodItemDetail = await querySession.LoadAsync<FoodItemDetail>(request.Id, cancellationToken);
        
        if (foodItemDetail is null || foodItem is null)
        {
           return Result.Failure<FoodItemDetail>(FoodItemErrors.NotFound);
        }
        if (foodItem.HouseholdId != currentHouseholdId)
        {
            return Result.Failure<FoodItemDetail>(FoodItemErrors.ForbiddenAccess);
        }
        return foodItemDetail;
    }
}
