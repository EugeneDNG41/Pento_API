using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;
using Pento.Domain.Units;

namespace Pento.Application.FoodReferences.Update;
internal sealed class UpdateFoodReferenceCommandHandler(
    IFoodReferenceRepository foodReferenceRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateFoodReferenceCommand, Guid>
{
    public async Task<Result<Guid>> Handle(UpdateFoodReferenceCommand request, CancellationToken cancellationToken)
    {
        FoodReference? foodRef = await foodReferenceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (foodRef is null)
        {
            return Result.Failure<Guid>(FoodReferenceErrors.NotFound);
        }

        if (!Enum.TryParse<FoodGroup>(request.FoodGroup, true, out FoodGroup foodGroup))
        {
            return Result.Failure<Guid>(FoodReferenceErrors.InvalidGroup);
        }
        Enum.TryParse<FoodDataType>(request.DataType, true, out FoodDataType dataType);
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Result.Failure<Guid>(FoodReferenceErrors.InvalidName);
        }
        if (!Enum.TryParse<UnitType>(request.UnitType, true, out UnitType unitType))
        {
            return Result.Failure<Guid>(FoodReferenceErrors.InvalidGroup);
        }


        foodRef.Update(
            name: request.Name,
            foodGroup: foodGroup,
            dataType: dataType,
            foodCategoryId: request.FoodCategoryId,
            brand: request.Brand,
            barcode: request.Barcode,
            usdaId: request.UsdaId,
            publishedOnUtc: request.PublishedOnUtc,
            typicalShelfLifeDaysPantry: request.TypicalShelfLifeDays_Pantry,
            typicalShelfLifeDaysFridge: request.TypicalShelfLifeDays_Fridge,
            typicalShelfLifeDaysFreezer: request.TypicalShelfLifeDays_Freezer,
            addedBy: null,
            imageUrl: request.ImageUrl,
            unitType: unitType,
            utcNow: DateTime.UtcNow
        );

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(foodRef.Id);
    }
}
