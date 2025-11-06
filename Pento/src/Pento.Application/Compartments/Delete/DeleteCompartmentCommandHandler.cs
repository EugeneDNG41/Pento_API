using Marten;
using Marten.Linq.SoftDeletes;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Compartments;
using Pento.Domain.FoodItems;

namespace Pento.Application.Compartments.Delete;

internal sealed class DeleteCompartmentCommandHandler(
    IUserContext userContext,
    IGenericRepository<Compartment> compartmentRepository,
    IQuerySession querySession,
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
        bool itemsInCompartment = await querySession.Query<FoodItem>()
            .Where(item => item.CompartmentId == compartment.Id && item.Quantity > 0)
            .AnyAsync(cancellationToken);
        if (itemsInCompartment)
        {
            return Result.Failure(CompartmentErrors.NotEmpty);
        }
        //should check if compartment is empty before deleting
        compartment.Delete();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
