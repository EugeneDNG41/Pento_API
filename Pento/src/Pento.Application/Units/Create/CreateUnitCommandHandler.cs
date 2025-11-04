using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Units;

namespace Pento.Application.Units.Create;
internal sealed class CreateUnitCommandHandler(
    IUnitRepository unitRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateUnitCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateUnitCommand request, CancellationToken cancellationToken)
    {
        Unit existingUnit = await unitRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existingUnit is not null)
        {
            return Result.Failure<Guid>(UnitErrors.Conflict);
        }

        if (!string.IsNullOrWhiteSpace(request.Abbreviation))
        {
            Unit? existingAbbr = await unitRepository.GetByAbbreviationAsync(request.Abbreviation, cancellationToken);
            if (existingAbbr is not null)
            {
                return Result.Failure<Guid>(UnitErrors.Conflict);
            }
        }

        var unit = Unit.Create(
            request.Name,
            request.Abbreviation,
            request.ToBaseFactor,
            request.Type
        );

        await unitRepository.AddAsync(unit, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(unit.Id);
    }
}
