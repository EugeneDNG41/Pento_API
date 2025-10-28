using Pento.Application.Abstractions.Email;
using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.Get;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;

namespace Pento.Application.Users.Register;

internal sealed class UserRegisteredDomainEventHandler(IQueryHandler<GetUserQuery, UserResponse> handler, IEmailService emailService)
    : DomainEventHandler<UserRegisteredDomainEvent>
{
    public override async Task Handle(
        UserRegisteredDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        Result<UserResponse> result = await handler.Handle(
            new GetUserQuery(domainEvent.UserId),
            cancellationToken);

        if (result.IsFailure)
        {
            throw new PentoException(nameof(GetUserQuery), result.Error);
        }

        await emailService.SendAsync(
            result.Value.Email,
            "Welcome to Pento!",
            $"Hello {result.Value.FirstName}, welcome to Pento!");
    }
}
