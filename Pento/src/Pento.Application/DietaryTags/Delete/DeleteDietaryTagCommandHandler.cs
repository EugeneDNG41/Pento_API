using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.DietaryTags;

namespace Pento.Application.DietaryTags.Delete;

internal sealed class DeleteDietaryTagCommandHandler(
    IGenericRepository<DietaryTag> dietaryTagRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteDietaryTagCommand, Guid>
{
    public async Task<Result<Guid>> Handle(DeleteDietaryTagCommand request, CancellationToken cancellationToken)
    {
        DietaryTag? tag = await dietaryTagRepository.GetByIdAsync(request.Id, cancellationToken);
        if (tag is null)
        {
            return Result.Failure<Guid>(DietaryTagErrors.NotFound);
        }

        await dietaryTagRepository.RemoveAsync(tag, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(tag.Id);
    }
}
