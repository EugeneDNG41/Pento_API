using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;

namespace Pento.Application.GiveawayPosts.Create;
internal sealed class CreateGiveawayPostCommandHandler() : ICommandHandler<CreateGiveawayPostCommand, Guid>
{
    public Task<Result<Guid>> Handle(CreateGiveawayPostCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
