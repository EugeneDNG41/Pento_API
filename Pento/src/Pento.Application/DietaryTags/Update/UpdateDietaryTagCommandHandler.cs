using System.Globalization;
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
        bool duplicateNameExists = await dietaryTagRepository
            .AnyAsync(
                dt => dt.Id != request.Id && dt.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase),
                cancellationToken);
        if (duplicateNameExists)
        {
            return Result.Failure<Guid>(DietaryTagErrors.DuplicateName);
        }
        tag.Update(request.Name, request.Description);
        await dietaryTagRepository.UpdateAsync(tag, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(tag.Id);
    }
}
