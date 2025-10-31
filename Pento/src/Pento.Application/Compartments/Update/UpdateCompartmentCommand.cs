
using FluentValidation;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Compartments;
using Pento.Domain.Households;
using Pento.Domain.Users;

namespace Pento.Application.Compartments.Update;

public sealed record UpdateCompartmentCommand(Guid Id, Guid UserHouseholdId, string Name, string? Notes) : ICommand;

internal sealed class UpdateCompartmentCommandValidator : AbstractValidator<UpdateCompartmentCommand>
{
    public UpdateCompartmentCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
        
        RuleFor(x => x.Notes)
            .MaximumLength(500);
    }
}
internal sealed class UpdateCompartmentCommandHandler(
    IGenericRepository<Compartment> compartmentRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateCompartmentCommand>
{
    public async Task<Result> Handle(UpdateCompartmentCommand command, CancellationToken cancellationToken)
    {
        Compartment? compartment = await compartmentRepository.GetByIdAsync(command.Id, cancellationToken);
        if (compartment == null)
        {
            return Result.Failure(CompartmentErrors.NotFound);
        }
        if (compartment.HouseholdId != command.UserHouseholdId)
        {
            return Result.Failure(UserErrors.NotInHouseHold);
        }
        compartment.Update(command.Name, command.Notes);
        compartmentRepository.Update(compartment);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
