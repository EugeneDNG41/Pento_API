using Pento.Domain.Abstractions;

namespace Pento.Application.Abstractions.Services;

public interface ICompartmentService
{
    Task<Result> CheckIfEmptyAsync(Guid compartmentId, Guid householdId, CancellationToken cancellationToken);
}
