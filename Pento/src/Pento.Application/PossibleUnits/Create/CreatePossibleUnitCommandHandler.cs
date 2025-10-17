using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.PossibleUnits;

namespace Pento.Application.PossibleUnits.Create;
internal sealed class CreatePossibleUnitCommandHandler(
    IPossibleUnitRepository possibleUnitRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreatePossibleUnitCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreatePossibleUnitCommand request, CancellationToken cancellationToken)
    {
        IReadOnlyList<PossibleUnit> existingUnits = await possibleUnitRepository.GetByFoodRefIdAsync(request.FoodRefId, cancellationToken);
        if (existingUnits.Any(p => p.UnitId == request.UnitId))
        {
            return Result.Failure<Guid>(PossibleUnitErrors.Conflict);
        }

        DateTime utcNow = DateTime.UtcNow;

        var possibleUnit = PossibleUnit.Create(
            request.UnitId,
            request.FoodRefId,
            request.IsDefault,
            utcNow
        );

        await possibleUnitRepository.AddAsync(possibleUnit, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(possibleUnit.Id);
    }
}
