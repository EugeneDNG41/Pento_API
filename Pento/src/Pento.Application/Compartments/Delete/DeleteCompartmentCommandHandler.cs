using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Domain.Abstractions;
using Pento.Domain.Compartments;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.FoodItems;
using Pento.Domain.Trades;

namespace Pento.Application.Compartments.Delete;

internal sealed class DeleteCompartmentCommandHandler(
    IUserContext userContext,
    ICompartmentService compartmentService,
    IGenericRepository<Compartment> compartmentRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteCompartmentCommand>
{
    public async Task<Result> Handle(DeleteCompartmentCommand request, CancellationToken cancellationToken)
    {
        Guid? currentHouseholdId = userContext.HouseholdId;
        Compartment? compartment = await compartmentRepository.GetByIdAsync(request.CompartmentId, cancellationToken);
        if (compartment is null)
        {
            return Result.Failure(CompartmentErrors.NotFound);
        }
        if (compartment.HouseholdId != currentHouseholdId)
        {
            return Result.Failure(CompartmentErrors.ForbiddenAccess);
        }
        Result checkEmptyResult = await compartmentService.CheckIfEmptyAsync(compartment.Id, currentHouseholdId.Value, cancellationToken);
        if (checkEmptyResult.IsFailure)
        {
            return checkEmptyResult;
        }
        bool otherCompartmentExists = await compartmentRepository.AnyAsync(c => c.HouseholdId == currentHouseholdId && c.Id != compartment.Id, cancellationToken);
        if (!otherCompartmentExists)
        {
            return Result.Failure(CompartmentErrors.AtLeastOne);
        }
        //should check if compartment is empty before deleting
        await compartmentRepository.RemoveAsync(compartment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
