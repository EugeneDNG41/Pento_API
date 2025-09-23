using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Common.Application.Messaging;
using Pento.Common.Domain;

namespace Pento.Modules.Community.Application.GiveawayPost.Create;
internal sealed class CreateGiveawayPostCommandHandler() : ICommandHandler<CreateGiveawayPostCommand, Guid>
{
    public Task<Result<Guid>> Handle(CreateGiveawayPostCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
