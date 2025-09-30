using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;

namespace Pento.Application.Comments.Get;
internal sealed class GetCommentQueryHandler() : IQueryHandler<GetCommentQuery, CommentResponse>
{
    public Task<Result<CommentResponse>> Handle(GetCommentQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
