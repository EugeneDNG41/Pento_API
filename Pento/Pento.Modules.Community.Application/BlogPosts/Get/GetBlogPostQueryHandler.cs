using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Common.Application.Messaging;
using Pento.Common.Domain;

namespace Pento.Modules.Community.Application.BlogPost.Get;
internal sealed class GetBlogPostQueryHandler() : IQueryHandler<GetBlogPostQuery, BlogPostResponse>
{
    public Task<Result<BlogPostResponse>> Handle(GetBlogPostQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
