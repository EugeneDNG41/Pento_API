using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;

namespace Pento.Application.Users.Delete;

public sealed record DeleteUserCommand(Guid UserId) : ICommand;

internal sealed class DeleteUserCommandHandler() : ICommandHandler<DeleteUserCommand>
{
    public async Task<Result> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        // Implementation to be added
        throw new NotImplementedException();
    }
}
