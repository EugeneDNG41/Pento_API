using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.DietaryTags;

namespace Pento.Application.DietaryTags.Update;

internal sealed class UpdateDietaryTagCommandHandler(
    IGenericRepository<DietaryTag> dietaryTagRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateDietaryTagCommand, Guid>
{
    public async Task<Result<Guid>> Handle(UpdateDietaryTagCommand request, CancellationToken cancellationToken)
    {
        DietaryTag? tag = await dietaryTagRepository.GetByIdAsync(request.Id, cancellationToken);
        if (tag is null)
        {
            return Result.Failure<Guid>(DietaryTagErrors.NotFound);
        }

        typeof(DietaryTag).GetProperty(nameof(DietaryTag.Name))?.SetValue(tag, request.Name);
        typeof(DietaryTag).GetProperty(nameof(DietaryTag.Description))?.SetValue(tag, request.Description);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(tag.Id);
    }
}
