
using Marten;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItems.Projections;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;


namespace Pento.Application.FoodItems.GetById;

internal sealed class GetFoodItemByIdQueryHandler(
    IQuerySession querySession)
    : IQueryHandler<GetFoodItemByIdQuery, FoodItemDetail>
{
    public async Task<Result<FoodItemDetail>> Handle(GetFoodItemByIdQuery request, CancellationToken cancellationToken)
    {

        FoodItemDetail? foodItem = await querySession.LoadAsync<FoodItemDetail>(request.Id, cancellationToken);
        if ( foodItem is null)
        {
           return Result.Failure<FoodItemDetail>(FoodItemErrors.NotFound);
        }
        return foodItem;
    }
}
