using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.ThirdPartyServices.Identity;
using Pento.Application.Users.Get;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;

namespace Pento.Application.Users.Register;

internal sealed class UserRegisteredDomainEventHandler(IIdentityProviderService identityProviderService)
    : DomainEventHandler<UserRegisteredDomainEvent>
{
    public override async Task Handle(
        UserRegisteredDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        Result result = await identityProviderService.SendVerificationEmailAsync(domainEvent.IdentityId, cancellationToken);

        if (result.IsFailure)
        {
            throw new PentoException(nameof(GetUserQuery), result.Error);
        }
    }
}
