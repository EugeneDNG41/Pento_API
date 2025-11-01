
using FluentValidation;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Compartments;
using Pento.Domain.Households;
using Pento.Domain.Users;

namespace Pento.Application.Compartments.Update;

public sealed record UpdateCompartmentCommand(Guid Id, string Name, string? Notes) : ICommand;

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
        compartment.Update(command.Name, command.Notes);
        compartmentRepository.Update(compartment);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
