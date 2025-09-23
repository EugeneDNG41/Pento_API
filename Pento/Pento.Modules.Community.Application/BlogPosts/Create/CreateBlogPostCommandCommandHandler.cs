using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pento.Common.Application.Messaging;
using Pento.Common.Domain;
using Pento.Modules.Community.Application.BlogPost.Create;


namespace Pento.Modules.Community.Application.BlogPosts.Create;
internal sealed class CreateBlogPostCommandCommandHandler(): ICommandHandler<CreateBlogPostCommand, Guid>
{
    public Task<Result<Guid>> Handle(CreateBlogPostCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

