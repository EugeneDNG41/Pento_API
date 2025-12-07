using Pento.Application.Abstractions.External.File;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Milestones;

namespace Pento.Application.Milestones.UpdateIcon;

internal sealed class UpdateMilestoneIconCommandHandler(
    IBlobService blobService,
    IGenericRepository<Milestone> milestoneRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateMilestoneIconCommand, Uri>
{
    public async Task<Result<Uri>> Handle(UpdateMilestoneIconCommand command, CancellationToken cancellationToken)
    {
        Milestone? milestone = await milestoneRepository.GetByIdAsync(command.MilestoneId, cancellationToken);
        if (milestone == null)
        {
            return Result.Failure<Uri>(MilestoneErrors.NotFound);
        }
        Result<Uri> urlResult = await blobService.UploadImageAsync(command.IconFile, nameof(Milestone), cancellationToken);
        if (urlResult.IsFailure)
        {
            return Result.Failure<Uri>(urlResult.Error);
        }
        if (milestone.IconUrl != null)
        {
            Result deleteResult = await blobService.DeleteImageAsync(nameof(Milestone), milestone.IconUrl.ToString(), cancellationToken);
            if (deleteResult.IsFailure)
            {
                return Result.Failure<Uri>(deleteResult.Error);
            }
        }
        milestone.UpdateIconUrl(urlResult.Value);
        milestoneRepository.Update(milestone);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return urlResult.Value;
    }
}

