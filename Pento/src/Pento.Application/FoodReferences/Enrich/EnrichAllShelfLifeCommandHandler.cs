using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.External.File;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;
using Pento.Domain.Units;

namespace Pento.Application.FoodReferences.Enrich;

internal sealed class EnrichAllShelfLifeCommandHandler(
    IDateTimeProvider dateTimeProvider,
    IGenericRepository<FoodReference> foodReferenceRepository,
    IFoodAiEnricher foodAiEnricher,
    IUnitOfWork unitOfWork) : ICommandHandler<EnrichAllShelfLifeCommand>
{
    public async Task<Result> Handle(EnrichAllShelfLifeCommand command, CancellationToken cancellationToken)
    {
        IEnumerable<FoodReference> foodReferences = await foodReferenceRepository.FindAsync(fr => 
        fr.TypicalShelfLifeDays_Freezer == 0 && 
        fr.TypicalShelfLifeDays_Fridge == 0 &&
        fr.TypicalShelfLifeDays_Pantry == 0, cancellationToken);
        foreach (FoodReference foodReference in foodReferences)
        {
            Result<ProductExtraInformationWithoutFoodGroup> extraInfoResult = await foodAiEnricher.EnrichAsync(foodReference.Name, cancellationToken);
            if (extraInfoResult.IsSuccess)
            {
                foodReference.UpdateShelfLifeDays(
                    extraInfoResult.Value.PantryShelfLife,
                    extraInfoResult.Value.FridgeShelfLife,
                    extraInfoResult.Value.FreezerShelfLife,
                    dateTimeProvider.UtcNow);
                foodReference.UpdateUnitType(
                    (UnitType)extraInfoResult.Value.UnitType,
                    dateTimeProvider.UtcNow);
                foodReferenceRepository.Update(foodReference);
            }
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
