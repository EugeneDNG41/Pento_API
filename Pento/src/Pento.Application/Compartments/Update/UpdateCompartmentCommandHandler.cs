
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Compartments;

namespace Pento.Application.Compartments.Update;

internal sealed class UpdateCompartmentCommandHandler(
    IUserContext userContext,
    IGenericRepository<Compartment> compartmentRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateCompartmentCommand>
{
    public async Task<Result> Handle(UpdateCompartmentCommand command, CancellationToken cancellationToken)
    {
        Guid? userHouseholdId = userContext.HouseholdId;
        Compartment? compartment = await compartmentRepository.GetByIdAsync(command.Id, cancellationToken);
        if (compartment == null)
        {
            return Result.Failure(CompartmentErrors.NotFound);
        }
        if (compartment.HouseholdId != userHouseholdId)
        {
            return Result.Failure(CompartmentErrors.ForbiddenAccess);
        }
        if (await compartmentRepository.AnyAsync(c => c.Name == command.Name && c.Id != compartment.Id && c.HouseholdId == userHouseholdId, cancellationToken))
        {
            return Result.Failure(CompartmentErrors.DuplicateName);
        }
        compartment.UpdateName(command.Name);
        compartment.UpdateNotes(command.Notes);
        compartmentRepository.Update(compartment);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
