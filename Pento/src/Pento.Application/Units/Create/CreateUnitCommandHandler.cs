using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Units;

namespace Pento.Application.Units.Create;
internal sealed class CreateUnitCommandHandler(
    IGenericRepository<Unit> unitRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateUnitCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateUnitCommand request, CancellationToken cancellationToken)
    {
        Unit existingUnit = (await unitRepository.FindAsync(u => u.Name == request.Name, cancellationToken)).FirstOrDefault();
        if (existingUnit is not null)
        {
            return Result.Failure<Guid>(UnitErrors.Conflict);
        }

        if (!string.IsNullOrWhiteSpace(request.Abbreviation))
        {
            Unit? existingAbbr = (await unitRepository.FindAsync(u => u.Abbreviation == request.Abbreviation, cancellationToken)).FirstOrDefault();
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

        unitRepository.Add(unit);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(unit.Id);
    }
}
