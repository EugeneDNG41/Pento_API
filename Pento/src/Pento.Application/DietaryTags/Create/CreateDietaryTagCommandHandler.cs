using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.DietaryTags;

namespace Pento.Application.DietaryTags.Create;

internal sealed class CreateDietaryTagCommandHandler(
    IGenericRepository<DietaryTag> dietaryTagRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateDietaryTagCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateDietaryTagCommand request, CancellationToken cancellationToken)
    {


        var tag = DietaryTag.Create(request.Name.Trim(), request.Description?.Trim());

        dietaryTagRepository.Add(tag);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(tag.Id);
    }
}
