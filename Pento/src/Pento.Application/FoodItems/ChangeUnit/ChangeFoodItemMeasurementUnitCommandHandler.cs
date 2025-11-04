using Marten;
using Marten.Events;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Converter;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodItems.Events;
using Pento.Domain.PossibleUnits;
using Pento.Domain.Units;

namespace Pento.Application.FoodItems.ChangeUnit;
#pragma warning disable CS9113 // Parameter is unread.
#pragma warning disable S125
internal sealed class ChangeFoodItemMeasurementUnitCommandHandler(
    IUserContext userContext,
    IDocumentSession session,
    IGenericRepository<PossibleUnit> possibleUnitRepository,
    IGenericRepository<Unit> unitRepository,
    IUnitConverter converter) : ICommandHandler<ChangeFoodItemMeasurementUnitCommand>
{
    public async Task<Result> Handle(ChangeFoodItemMeasurementUnitCommand command, CancellationToken cancellationToken)
    {
        IEventStream<FoodItem> stream = await session.Events.FetchForWriting<FoodItem>(command.Id, command.Version, cancellationToken);
        FoodItem? foodItem = stream.Aggregate;
        if (foodItem is null)
        {
            return Result.Failure(FoodItemErrors.NotFound);
        }
        if (foodItem.HouseholdId != userContext.HouseholdId)
        {
            return Result.Failure(FoodItemErrors.ForbiddenAccess);
        }
        if (foodItem.UnitId == command.MeasurementUnitId)
        {
            return Result.Success();
        }
        //bool possible = await possibleUnitRepository.AnyAsync(
        //    pu => pu.FoodReferenceId == foodItem.FoodReferenceId && pu.UnitId == command.MeasurementUnitId,
        //    cancellationToken);
        //if (!possible)
        //{
        //    return Result.Failure(FoodItemErrors.InvalidMeasurementUnit);
        //}
        Result<decimal> convertedResult = await converter.ConvertAsync(foodItem.Quantity, foodItem.UnitId, command.MeasurementUnitId, cancellationToken);
        if (convertedResult.IsFailure)
        {
            return Result.Failure(convertedResult.Error);
        }
        Unit unit = await unitRepository.GetByIdAsync(command.MeasurementUnitId, cancellationToken);
        await session.Events.AppendOptimistic(command.Id, new FoodItemUnitChanged(command.MeasurementUnitId, convertedResult.Value, unit!.Abbreviation));
        session.LastModifiedBy = userContext.UserId.ToString();
        await session.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
