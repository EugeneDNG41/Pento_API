using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Features;

namespace Pento.Application.Features.Update;

internal sealed class UpdateFeatureCommandHandler(
    IGenericRepository<Feature> featureRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateFeatureCommand>
{
    public async Task<Result> Handle(UpdateFeatureCommand command, CancellationToken cancellationToken)
    {
        Feature? feature = await featureRepository.GetByIdAsync(command.Code, cancellationToken);
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
