using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Common.Application.Messaging;
using Pento.Common.Domain;

namespace Pento.Modules.Community.Application.Comment.Create;
public sealed class CreateCommentCommandHandler() : ICommandHandler<CreateCommentCommand, Guid>
{   public Task<Result<Guid>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

