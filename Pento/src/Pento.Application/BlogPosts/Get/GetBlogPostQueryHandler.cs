using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;

namespace Pento.Application.BlogPosts.Get;
internal sealed class GetBlogPostQueryHandler() : IQueryHandler<GetBlogPostQuery, BlogPostResponse>
{
    public Task<Result<BlogPostResponse>> Handle(GetBlogPostQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
