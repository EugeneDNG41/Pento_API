using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;


namespace Pento.Application.BlogPosts.Create;
internal sealed class CreateBlogPostCommandCommandHandler() : ICommandHandler<CreateBlogPostCommand, Guid>
{
    public Task<Result<Guid>> Handle(CreateBlogPostCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
