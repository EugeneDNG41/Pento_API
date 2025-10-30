using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;

namespace Pento.Application.Households.Create;

internal sealed class CreateHouseholdCommandHandler(IGenericRepository<Household> repository, IUnitOfWork unitOfWork) : ICommandHandler<CreateHouseholdCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateHouseholdCommand command, CancellationToken cancellationToken)
    {
        var household = Household.Create(command.Name);
        repository.Add(household);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return household.Id;
    }
}
