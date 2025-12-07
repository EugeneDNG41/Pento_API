using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;

namespace Pento.Application.FoodReferences.Create;

internal sealed class CreateFoodReferenceCommandHandler(
    IGenericRepository<FoodReference> foodReferenceRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateFoodReferenceCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateFoodReferenceCommand request,
        CancellationToken cancellationToken)
    {
        DateTime utcNow = DateTime.UtcNow;

        var foodReference = FoodReference.Create(
            request.Name,
            request.FoodGroup,
            request.FoodCategoryId,
            request.Brand,
            request.Barcode,
            request.UsdaId,
            request.TypicalShelfLifeDays_Pantry,
            request.TypicalShelfLifeDays_Fridge,
            request.TypicalShelfLifeDays_Freezer,
            request.AddedBy,
            request.ImageUrl,
            request.UnitType,
            utcNow
        );
        foodReferenceRepository.Add(foodReference);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(foodReference.Id);
    }
}
