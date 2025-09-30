using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;

namespace Pento.Application.GiveawayClaims.Create;
internal sealed class CreateGiveawayClaimCommandHandler() : ICommandHandler<CreateGiveawayClaimCommand, Guid>
{
    public Task<Result<Guid>> Handle(CreateGiveawayClaimCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
