using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;

namespace Pento.Application.Comments.Create;

public sealed class CreateCommentCommandHandler() : ICommandHandler<CreateCommentCommand, Guid>
{
    public Task<Result<Guid>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

