using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Common.Application.Messaging;
using Pento.Common.Domain;

namespace Pento.Modules.Community.Application.GiveawayClaim.Create;
internal class CreateGiveawayClaimCommandCommandHandler() : ICommandHandler<CreateGiveawayClaimCommand, Guid>
{
    public Task<Result<Guid>> Handle(CreateGiveawayClaimCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
