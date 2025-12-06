using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Features;
using Pento.Domain.Shared;

namespace Pento.Application.Features.Update;

internal sealed class UpdateFeatureCommandHandler(
    IGenericRepository<Feature> featureRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateFeatureCommand>
{
    public async Task<Result> Handle(UpdateFeatureCommand command, CancellationToken cancellationToken)
    {
        Feature? feature = (await featureRepository.FindAsync(f => f.Code == command.Code, cancellationToken)).SingleOrDefault();
        if (feature is null)
        {
            return Result.Failure(FeatureErrors.NotFound);
        }
        if (command.Name == feature.Name)
        {
            return Result.Failure(FeatureErrors.NameTaken);
        }

        feature.UpdateDetails(command.Name, command.Description, command.DefaultQuota, command.DefaultResetPeriod);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
